using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;
using System.Runtime.Serialization;

namespace FlowOptions.EggOn.ModuleCore.ViewModels
{
    [DataContract(Name = "Message", Namespace = "http://www.eggon.pt/EggOn.Service.Models")]
    public class ServiceMessage
    {
        [DataMember(Order = 1, IsRequired = true)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        [JsonConverter(typeof(StringEnumConverter))]
        public HttpStatusCode? HttpStatusCode { get; set; }

        [DataMember(Order = 3, IsRequired = true)]
        public string Message { get; set; }
    }

    [DataContract]
    public enum MessageType
    {
        [EnumMember(Value = "Information")]
        Information,
        [EnumMember(Value = "Error")]
        Error
    }
}