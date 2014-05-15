using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlowOptions.EggOn.Base.ViewModels
{
    [DataContract(Name = "Layout", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class LayoutDto
    {
        [DataMember]
        public Guid? LogoId { get; set; }

        [DataMember]
        public string BarBackColor { get; set; }

        [DataMember]
        public string BarButtonColor { get; set; }

        [DataMember]
        public string BackColor { get; set; }
    }
}