using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlowOptions.EggOn.Data.ViewModels
{
    [DataContract(Name = "Field", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class FieldDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public bool PrimaryKey { get; set; }

        [DataMember]
        public Guid FieldTypeId { get; set; }

        [DataMember]
        public int Order { get; set; }

        [DataMember]
        public string DefaultValue { get; set; }

        [DataMember]
        public bool ShowInList { get; set; }
    }
}