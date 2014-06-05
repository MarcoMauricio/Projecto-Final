using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Context.Models
{
    public class Category
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string CategoryName { get; set; }
        public ObjectId documentID { get; set; }
    }
}
