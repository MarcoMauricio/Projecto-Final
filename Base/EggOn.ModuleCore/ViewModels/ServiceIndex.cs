using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlowOptions.EggOn.ModuleCore.ViewModels
{
    [DataContract(Name = "EggOn", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class ServiceIndex
    {
        [DataMember(Order = 1, IsRequired = true)]
        public string Message { get; set; }

        [DataMember(Order = 2, IsRequired = true)]
        public string Version { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string CurrentUser { get; set; }

        [DataMember(Order = 4, IsRequired = true)]
        public string[] LoadedModules { get; set; }
    }
}