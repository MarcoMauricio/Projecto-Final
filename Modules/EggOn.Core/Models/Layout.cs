using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;

namespace FlowOptions.EggOn.Base.Models
{
    [TableName("[EggOn].[CoreLayout]"), PrimaryKey("Id", autoIncrement = false)]
    public class Layout
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }

        [AllowNull]
        public Guid? LogoId { get; set; }

        [AllowNull]
        public string BarBackColor { get; set; }

        [AllowNull]
        public string BarButtonColor { get; set; }

        [AllowNull]
        public string BackColor { get; set; }
    }
}