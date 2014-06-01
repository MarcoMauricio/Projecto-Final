using Context.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
namespace Context.DataAccessLayer
{
    public class EntityService
    {
        private readonly MongoHelper<Entity> _entities;

        public EntityService()
        {
            _entities = new MongoHelper<Entity>();
        }

        public void Create(Entity Entity)
        {
            _entities.Collection.Save(Entity);
        }

        public void Edit(Entity Entity)
        {
            _entities.Collection.Update(
                Query.EQ("_id", Entity.Id),
                Update.Set("TableName", Entity.TableName)
                    .Set("EntityName", Entity.EntityName)
                    .Set("CategoryID", Entity.CategoryID)
                    .Set("IndexTable", Entity.IndexTable));
        }

        public void Delete(ObjectId EntityId)
        {
            _entities.Collection.Remove(Query.EQ("_id", EntityId));
        }

        public MongoCursor<Entity> GetEntities()
        {
            return _entities.Collection.FindAll();
        }

        public Entity GetEntity(ObjectId id)
        {
            return _entities.Collection.FindOne(Query.EQ("_id", id));
        }
    }
}
