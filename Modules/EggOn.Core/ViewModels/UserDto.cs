using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FlowOptions.EggOn.Base.ViewModels
{
    [DataContract(Name = "User", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class UserDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Email { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Password { get; set; }

        [DataMember]
        public Guid? InterfaceLanguageId { get; set; }

        [DataMember]
        public List<RoleDto> Roles { get; set; }

        [DataMember]
        public bool Validated { get; set; }

        [DataMember]
        public DateTime LastAction { get; set; }
    }
}