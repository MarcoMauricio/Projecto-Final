using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggOn.Context.Models
{
    public class Category
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string CategoryName { get; set; }
        public ObjectId documentID { get; set; }
    }
}
