using FlowOptions.EggOn.Base;
using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;

namespace FlowOptions.EggOn.Files.Models
{
    [TableName("[EggOn].[FilesRepositories]"), PrimaryKey("Id", autoIncrement = false)]
    public class Repository
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Size { get; set; }

        public int Type { get; set; }

        public Repository()
        {
            Type = 1; // Local
        }
    }
}