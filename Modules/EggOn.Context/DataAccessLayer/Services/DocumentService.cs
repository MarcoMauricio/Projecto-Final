using Context.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;


namespace Context.DataAccessLayer.Services
{
    /// <summary>
    /// Serviço para o acesso a documentos na base de dados
    /// </summary>

    public class DocumentService
    {
        private readonly MongoHelper<Document> _documents;
        public DocumentService()
        {
            _documents = new MongoHelper<Document>();
        }
        public MongoCursor<Document> GetDocuments()
        {
            return _documents.Collection.FindAll();
        }

        public void CreateDocument(Document document)
        {
            document.Entities = new List<Entity>();

            _documents.Collection.Save(document);
        }

        public void Delete(ObjectId documentId)
        {
            _documents.Collection.Remove(Query.EQ("_id", documentId));
        }

        public Document GetDocument(ObjectId documentId)
        {
            return _documents.Collection.FindOne(Query.EQ("_id", documentId));
        }
    }
}
