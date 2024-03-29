﻿using System.Collections.Generic;
using EggOn.Context.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace EggOn.Context.DataAccessLayer.Services
{
    class EntitiesService
    {
        private readonly MongoHelper<Document> _documents;

        public EntitiesService()
        {
            _documents = new MongoHelper<Document>();
        }

        public void AddEntity(ObjectId documentId, Entity entity)
        {
            _documents.Collection.Update(Query.EQ("_id", documentId),
                Update.PushWrapped("Entities", entity));
        }

        public List<Entity> GetEntities(ObjectId documentId)
        {
            return _documents.Collection.FindOne(Query.EQ("_id", documentId)).Entities;
        }
    }
}
