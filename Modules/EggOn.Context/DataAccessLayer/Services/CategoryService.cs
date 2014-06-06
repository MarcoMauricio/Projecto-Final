using Context.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;

namespace Context.DataAccessLayer.Services
{
    class CategoryService
    {
        private readonly MongoHelper<Document> _documents;

        public CategoryService ()
        {
            _documents = new MongoHelper<Document>();
        }

        public void AddCategory(ObjectId documentId, Category category)
        {
            _documents.Collection.Update(Query.EQ("_id", documentId),
                Update.PushWrapped("Category", category));
        }
        public Category GetCategory(ObjectId documentId)
        {
            return null;// _documents.Collection.FindOne(Query.EQ("_id", documentId)).Category;
        }
    }
}
