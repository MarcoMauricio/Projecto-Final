using FlowOptions.EggOn.Base.Controllers;
using FlowOptions.EggOn.Data.Models;
using FlowOptions.EggOn.Data.ViewModels;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq;

namespace FlowOptions.EggOn.Data.Controllers
{
    public class RecordsController : EggOnApiController
    {
        [Route("data/containers/{containerId:guid}/records"), HttpGet]
        public List<Dictionary<string, object>> GetRecords(Guid containerId)
        {
            var container = this.Database.SingleOrDefault<Container>(containerId);

            if (container == null)
            {
                throw NotFound("Container not found.");
            }

            if (container.Type != ContainerTypes.Local)
            {
                throw BadRequest("This module currently only supports local containers.");
            }

            var fields = container.Fields;
            var data = container.Records;

            return data.Select(d => ((IDictionary<string, object>)d).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)).ToList();
        }

        [Route("data/containers/{containerId:guid}/records"), HttpPost]
        public dynamic CreateRecord(Guid containerId, ExpandoObject data)
        {
            var container = this.Database.SingleOrDefault<Container>(containerId);

            if (container == null)
            {
                throw NotFound("Container not Found.");
            }

            var fields = container.Fields;
            var primaryField = fields.FirstOrDefault(f => f.PrimaryKey);

            dynamic newRecord = data as ExpandoObject;

            // TODO: Verify fields.

            this.Database.Insert(container.TableName, primaryField.ColumnName, false, newRecord);

            return data;
        }

        [Route("data/containers/{containerId:guid}/records/{recordId}"), HttpGet]
        public dynamic GetRecord(Guid containerId, string recordId)
        {
            Container container = this.Database.SingleOrDefault<Container>(containerId);

            if (container == null)
            {
                throw NotFound("Container not Found.");
            }

            var fields = container.Fields;
            var primaryField = fields.FirstOrDefault(f => f.PrimaryKey);

            dynamic record = this.Database.SingleOrDefault<dynamic>("SELECT * FROM " + container.TableName + " WHERE " + primaryField.ColumnName +  " = @0", recordId);
            
            if (record == null)
            {
                throw NotFound("Record not Found.");
            }

            return record;
        }

        [Route("data/containers/{containerId:guid}/records/{recordId}"), HttpPut]
        public dynamic PutRecord(Guid containerId, string recordId, ExpandoObject data)
        {
            Container container = this.Database.SingleOrDefault<Container>(containerId);

            if (container == null)
            {
                throw NotFound("Container not Found.");
            }

            var fields = container.Fields;
            var primaryField = fields.FirstOrDefault(f => f.PrimaryKey);

            dynamic record = this.Database.SingleOrDefault<dynamic>("SELECT * FROM " + container.TableName + " WHERE " + primaryField.ColumnName + " = @0", recordId);

            if (record == null)
            {
                throw NotFound("Record not Found.");
            }

            // TODO: Verify fields.

            this.Database.Update(container.TableName, primaryField.ColumnName, data, recordId);

            return data;
        }

        [Route("data/containers/{containerId:guid}/records/{recordId}"), HttpDelete]
        public dynamic DeleteRecord(Guid containerId, string recordId)
        {
            Container container = this.Database.SingleOrDefault<Container>(containerId);

            if (container == null)
            {
                throw NotFound("Container not Found.");
            }

            var fields = container.Fields;
            var primaryField = fields.FirstOrDefault(f => f.PrimaryKey);

            dynamic record = this.Database.SingleOrDefault<dynamic>("SELECT * FROM " + container.TableName + " WHERE " + primaryField.ColumnName + " = @0", recordId);
            
            if (record == null)
            {
                throw NotFound("Record not Found.");
            }

            this.Database.Delete(container.TableName, primaryField.ColumnName, record, recordId);

            return record;
        }
    }
}
