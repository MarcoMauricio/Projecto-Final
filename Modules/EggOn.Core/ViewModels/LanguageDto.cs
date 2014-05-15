using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlowOptions.EggOn.Base.ViewModels
{
    [DataContract(Name = "Language", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class LanguageDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public string Code { get; set; }
    }
}