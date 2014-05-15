using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlowOptions.EggOn.Data.Models
{
    [TableName("[EggOn].[DataFieldTypes]"), PrimaryKey("Id", autoIncrement = false)]
    public class FieldType 
    {

        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }
        
        public string Name { get; set; }

        public string SqlType { get; set; }

        public bool CanBePrimary { get; set; }
    }
}
