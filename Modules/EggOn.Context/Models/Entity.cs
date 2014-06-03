﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Context.Models
{
    public class Entity
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string entityName { get; set; }
        public ObjectId documentID { get; set; }
    }
}
