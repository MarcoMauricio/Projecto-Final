

using Context.Models;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
namespace Context.DataAccessLayer.Services
{
    class SummaryService
    {
        private readonly MongoHelper<Document> _documents;

        public SummaryService ()
        {
            _documents = new MongoHelper<Document>();
        }

        public void AddSummary(ObjectId documentId, Summary summary)
        {
            _documents.Collection.Update(Query.EQ("_id", documentId),
                Update.PushWrapped("Summary", summary));
        }
    }
}
