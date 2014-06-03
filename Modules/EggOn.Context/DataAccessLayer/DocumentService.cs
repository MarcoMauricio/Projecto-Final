using Context.Models;
using EggOn.Context.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;


namespace Context.DataAccessLayer
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
            document.Summary = new Summary();
            document.Entities = new List<Entity>();
            document.Category = new Category();
            _documents.Collection.Insert(document);
        }

        public void Delete(ObjectId documentId)
        {
            _documents.Collection.Remove(Query.EQ("_id", documentId));
        }
    }
}
