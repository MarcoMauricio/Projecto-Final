using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Context.Models
{
    public class Entity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string TableName { get; set; }
        public string IndexTable { get; set; }
        public string EntityName { get; set; }
        public string CategoryID { get; set; }
    }
}
