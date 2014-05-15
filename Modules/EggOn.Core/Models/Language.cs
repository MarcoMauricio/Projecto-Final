using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;

namespace FlowOptions.EggOn.Base.Models
{
    [TableName("[EggOn].[CoreLanguages]"), PrimaryKey("Id", autoIncrement = false)]
    public class Language
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}