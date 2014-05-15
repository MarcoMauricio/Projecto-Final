using FlowOptions.EggOn.Base;
using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;

namespace FlowOptions.EggOn.Context.Models
{
    [TableName("[EggOn].[DocumentDocument]"), PrimaryKey("Id", autoIncrement = false)]

    public class Document
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }

        public string Text { get; set; }

        public string SummarizedText { get; set; }
  
        public DateTime? SubmitDateTime { get; set; }

        public List<string> ExtractedEntities { get; set; }
    }
}
