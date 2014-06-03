

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Context.Models
{
    public class Summary
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string SummaryText { get; set; }
        public ObjectId documentID { get; set; }

    }
}
