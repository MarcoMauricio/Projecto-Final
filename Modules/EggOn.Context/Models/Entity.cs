using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EggOn.Context.Models
{
    public class Entity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string EntityName { get; set; }
    }
}
