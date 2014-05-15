using System;
using System.Runtime.Serialization;

namespace FlowOptions.EggOn.Files.ViewModels
{
    [DataContract(Name = "Repository", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class RepositoryDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 3)]
        public int Type { get; set; }
    }
}