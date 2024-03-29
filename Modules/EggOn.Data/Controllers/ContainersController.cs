﻿using AutoMapper;
using FlowOptions.EggOn.Base;
using FlowOptions.EggOn.Base.Controllers;
using FlowOptions.EggOn.Data.Models;
using FlowOptions.EggOn.Data.ViewModels;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace FlowOptions.EggOn.Data.Controllers
{
    public class ContainersController : EggOnApiController
    {
        [Route("data/containers"), HttpGet]
        public List<ContainerDto> GetAllContainers()
        {
            var containers = Database.All<Container>();

            return Mapper.Map<List<ContainerDto>>(containers);
        }

        [Route("data/containers"), HttpPost]
        public ContainerDto CreateContainer(ContainerDto data)
        {
            var container = Mapper.Map<Container>(data);
            container.Id = GuidComb.NewGuid();
            container.TableName = GenerateTableName(container);

            if (data.Fields == null || data.Fields.Count == 0)
            {
                throw BadRequest("Data containers need to have fields.");
            }

            var fields = data.Fields.Select(dto => {
                var field = Mapper.Map<Field>(dto);
                field.Id = GuidComb.NewGuid();
                field.ContainerId = container.Id;
                field.ColumnName = GenerateColumnName(field);
                return field;
            }).ToList();

            using(var tr = Database.GetTransaction())
            {
                Database.Insert(container);

                foreach (var field in fields)
                {
                    Database.Insert(field);
                }

                CreateSqlTableFromContainer(container, fields);

                tr.Complete();
            }

            return Mapper.Map<ContainerDto>(container);
        }

        [Route("data/containers/{containerId:guid}"), HttpGet]
        public ContainerDto GetContainer(Guid containerId)
        {
            var container = Database.SingleOrDefault<Container>(containerId);

            if (container == null)
            {
                throw NotFound("Data container not found.");
            }

            return Mapper.Map<ContainerDto>(container);
        }

        [Route("data/containers/{containerId:guid}"), HttpPut]
        public ContainerDto UpdateContainer(Guid containerId, ContainerDto data)
        {
            var container = Database.SingleOrDefault<Container>(containerId);

            if (container == null)
            {
                throw NotFound("Data container not found.");
            }

            Mapper.Map<ContainerDto, Container>(data, container);
            container.Id = containerId;

            Database.Update(container);

            return Mapper.Map<ContainerDto>(container);
        }

        [Route("data/containers/{containerId:guid}"), HttpDelete]
        public ContainerDto DeleteContainer(Guid containerId)
        {
            var container = Database.SingleOrDefault<Container>(containerId);

            if (container == null)
            {
                throw NotFound("Data container not found.");
            }

            using (var tr = Database.GetTransaction())
            {
                Database.Delete<Field>("WHERE ContainerId = @0", container.Id);

                Database.Delete(container);

                DeleteSqlTableFromContainer(container);

                tr.Complete();
            }

            return Mapper.Map<ContainerDto>(container);
        }


        private string GenerateTableName(Container container)
        {
            var rgx = new Regex("[^a-zA-Z0-9_-]");

            var tableName = rgx.Replace(container.Name.Replace(' ', '_'), "");

            var testTableName = tableName;

            var counter = 1;
            while (Database.Exists<Container>("TableName = @0 AND Id <> @1", testTableName, container.Id))
            {
                testTableName = tableName + (counter++);
            }

            tableName = testTableName;

            if (container.Type == ContainerTypes.Local)
            {
                return "[Data].[" + tableName + "]";
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private string GenerateColumnName(Field field)
        {
            var rgx = new Regex("[^a-zA-Z0-9_-]");

            return rgx.Replace(field.Name.Replace(' ', '_'), "");
        }


        private void CreateSqlTableFromContainer(Container newContainer, List<Field> data)
        {
            var tableName = newContainer.TableName;

            var columns = new List<SqlColumn>();

            foreach (var field in data)
            {
                columns.Add(new SqlColumn()
                {
                    Name = field.ColumnName,
                    Type = field.Type.SqlType,
                    DefaultValue = field.DefaultValue,
                    PrimaryKey = field.PrimaryKey
                });
            }

            Database.CreateOrUpdateTable(tableName, columns);
        }

        private void UpdateSqlTableFromContainer(Container oldContainer, List<Field> oldFields, Container newContainer, List<Field> newFields)
        {
            if (oldContainer.TableName != newContainer.TableName)
            {
                Database.Execute("sp_RENAME @0, @1", oldContainer.TableName, newContainer.TableName.Substring("[Data].[".Length, newContainer.TableName.Length - "[Data].[".Length + 1));
            }

            foreach (var newField in newFields)
            {
                var oldField = oldFields.Where(c => c.Id == newField.Id && newField.Id != Guid.Empty).SingleOrDefault();

                // Is original child item with same ID in DB?
                if (oldField != null && oldField.Id != Guid.Empty)
                {
                    // Yes -> Rename column and change type.
                    if (newField.Name != oldField.Name)
                    {
                        Database.Execute("sp_RENAME @0, @1, 'COLUMN'", newContainer.TableName + "." + oldField.ColumnName, GenerateColumnName(newField));
                    }

                    if (newField.FieldTypeId != oldField.FieldTypeId)
                    {
                        Database.Execute("ALTER TABLE " + newContainer.TableName + " ALTER COLUMN " + newField.ColumnName + " " + newField.Type.SqlType);
                    }
                }
                else
                {
                    // No -> It's a new child item -> Create Column
                    Database.Execute("ALTER TABLE " + newContainer.TableName + " ADD " + newField.ColumnName + " " + newField.Type.SqlType);
                }
            }

            foreach (var oldField in oldFields.Where(c => c.Id != null && c.Id != Guid.Empty).ToList())
            {
                // Are there child items in the DB which are NOT in the
                // new child item collection anymore?
                if (!newFields.Any(c => c.Id == oldField.Id))
                {
                    // Yes -> It's a deleted child item -> Remove Column
                    Database.Execute("ALTER TABLE " + newContainer.TableName + " DROP COLUMN " + oldField.ColumnName);
                }
            }
        }

        private void DeleteSqlTableFromContainer(Container container)
        {
            Database.DropTable(container.TableName);
        }
    }
}
