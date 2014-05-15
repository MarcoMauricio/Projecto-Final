using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlowOptions.EggOn.Data.ViewModels
{
    [DataContract(Name = "FieldType", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class FieldTypeDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string SqlType { get; set; }

        [DataMember]
        public bool CanBePrimary { get; set; }
    }
}