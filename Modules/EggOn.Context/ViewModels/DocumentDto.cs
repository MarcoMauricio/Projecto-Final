using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FlowOptions.EggOn.Context.ViewModels
{
    [DataContract(Name = "Document", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]

    public class DocumentDto
    {
       
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Text { get; set; }

        [DataMember(Order = 3)]
        public string SummarizedText { get; set; }


        [DataMember(Order = 4)]
        public List<string> Entities { get; set; }
    }
}
