using Context;
using Context.DataAccessLayer;
using Context.DataAccessLayer.Services;
using Context.Models;
using Context.NLP.Services;
using FlowOptions.EggOn.Base.Controllers;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace FlowOptions.EggOn.Context.Controllers
{
    public class ContextController : EggOnApiController
    {
        static DocumentService documentsHelper = new DocumentService();
        [Route("context"), HttpGet]
        public List<Document> GetAllDocuments()
        {
            List<Document> list = new List<Document>();
            var cursor = documentsHelper.GetDocuments();
            foreach (Document doc in cursor)
            {
                list.Add(doc);
            }
            return list;
        }


        [Route("context"), HttpPost]
        public void CreateContext(string filePath, string tableName , string tableIndex)
        {
            ContextCore context = new ContextCore(new RemoteService(), filePath);
            MinedObject item = context.GetContext();
            Document doc = new Document();
            doc.Id = ObjectId.GenerateNewId();
            doc.TableIndex = tableIndex;
            doc.TableName = tableName;

            documentsHelper.CreateDocument(doc);
        }
    }
}
