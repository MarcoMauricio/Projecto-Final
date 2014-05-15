using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;

namespace FlowOptions.EggOn.Base.Models
{
    [TableName("[EggOn].[CoreRoles]"), PrimaryKey("Id", autoIncrement = false)]
    public class Role
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
