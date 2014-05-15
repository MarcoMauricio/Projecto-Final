using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlowOptions.EggOn.Data.ViewModels
{
    [DataContract(Name = "Container", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class ContainerDto
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public Guid? ParentContainerId { get; set; }

        [DataMember]
        public bool SingleRecord { get; set; }

        [DataMember]
        public List<FieldDto> Fields { get; set; }
    }
}