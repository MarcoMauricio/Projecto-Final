using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FlowOptions.EggOn.Files.ViewModels
{
    [DataContract(Name = "File", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class FileDto
    {
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }


        [DataMember(Order = 3)]
        public int Size { get; set; }

        [DataMember]
        public string ContentType { get; set; }

        [DataMember]
        public Guid? ParentFileId { get; set; }

        [DataMember]
        public int Type { get; set; }

        //[DataMember(EmitDefaultValue = false)]
        //public List<FileDto> Children { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid? RepositoryId { get; set; }


        [DataMember]
        public Guid UserId { get; set; }

        [DataMember]
        public DateTime UploadDate { get; set; }
    }
}