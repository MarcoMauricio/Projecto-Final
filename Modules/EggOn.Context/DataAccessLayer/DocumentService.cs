using Context.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
namespace Context.DataAccessLayer
{
    public class DocumentService
    {
        private readonly MongoHelper<Document> _documents;

        public DocumentService()
        {
            _documents = new MongoHelper<Document>();
        }

        public void Create(Document document)
        {
            _documents.Collection.Save(document);
        }

        public void Edit(Document document)
        {
            throw new NotSupportedException();
        }

        public void Delete(ObjectId documentId)
        {
            _documents.Collection.Remove(Query.EQ("_id", documentId));
        }

        public MongoCursor<Document> GetDocuments()
        {
            return _documents.Collection.FindAll();
        }

        public Document GetDocument(string id)
        {
            return _documents.Collection.FindOne(Query.EQ("_id", id));
        }
    }
}

