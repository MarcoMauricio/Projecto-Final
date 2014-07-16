using FlowOptions.EggOn.Base;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;

namespace FlowOptions.EggOn.Data.Models
{
    [TableName("[EggOn].[DataFields]"), PrimaryKey("Id", autoIncrement = false)]
    public class Field
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ColumnName { get; set; }

        [Constraint(typeof(FieldType))]
        public Guid FieldTypeId { get; set; }

        public bool PrimaryKey { get; set; }
        
        public int Order { get; set; }

        public bool ShowInList { get; set; }

        [AllowNull]
        public string DefaultValue { get; set; }
        
        [Constraint(typeof(Container))]
        public Guid ContainerId { get; set; }

        // Helper fields.

        [Ignore]
        public FieldType Type
        {
            get
            {
                using (var database = new EggOnDatabase())
                {
                    return database.SingleOrDefault<FieldType>(FieldTypeId);
                }
            }
        }

        [Ignore]
        public Container Container
        {
            get
            {
                using (var database = new EggOnDatabase())
                {
                    return database.SingleOrDefault<Container>(ContainerId);
                }
            }
        }
    }
}