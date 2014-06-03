using Context.Models;
using MongoDB.Bson;
namespace Context.DataAccessLayer.Services
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
    }
}
