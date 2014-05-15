using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FlowOptions.EggOn.Base.ViewModels
{
    [DataContract(Name = "Role", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class RoleDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }
    }
}