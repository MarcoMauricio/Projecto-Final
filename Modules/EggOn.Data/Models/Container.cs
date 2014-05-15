using FlowOptions.EggOn.Base;
using FlowOptions.EggOn.Data.ViewModels;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace FlowOptions.EggOn.Data.Models
{
    [TableName("[EggOn].[DataContainers]"), PrimaryKey("Id", autoIncrement = false)]
    public class Container
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }

        public string Name { get; set; }

        [Unique]
        public string TableName { get; set; }

        public ContainerTypes Type { get; set; }

        [AllowNull, Constraint(typeof(Container))]
        public Guid? ParentContainerId { get; set; }

        public bool SingleRecord { get; set; }

        [Ignore]
        public List<Field> Fields
        {
            get
            {
                using (var database = new EggOnDatabase())
                {
                    return database.Fetch<Field>("WHERE ContainerId = @0", this.Id);
                }
            }
        }

        [Ignore]
        public List<dynamic> Records
        {
            get
            {
                if (this.Type != ContainerTypes.Local)
                {
                    throw new NotImplementedException();
                }

                using (var database = new EggOnDatabase())
                {
                    return database.Fetch<dynamic>("SELECT * FROM " + database.CleanTableName(this.TableName));
                }
            }
        }

        [Ignore]
        public Container ParentContainer 
        {
            get
            {
                using (var database = new EggOnDatabase())
                {
                    return database.SingleOrDefault<Container>(this.ParentContainerId);
                }
            }
        }

        [Ignore]
        public List<Container> ChildContainers
        {
            get
            {
                using (var database = new EggOnDatabase())
                {
                    return database.Fetch<Container>("WHERE ParentContainerId = @0", this.Id);
                }
            }
        }
    }

    public enum ContainerTypes
    {
        Local = 1,
        Dropbox = 2
    }
}