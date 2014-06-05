using Context;
using Context.DataAccessLayer.Services;
using Context.Models;
using Context.NLP.Services;
using FlowOptions.EggOn.Base.Controllers;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlowOptions.EggOn.Context.Controllers
{
    public class ContextController : EggOnApiController
    {
        DocumentService documentsHelper;
        public ContextController()
        {
            this.documentsHelper = new DocumentService();
        }
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
        public HttpResponseMessage CreateContext(aux cenas)
        {
            ContextCore context = new ContextCore(new RemoteService(), cenas.filePath);
            MinedObject item = context.GetContext();
            Document doc = new Document();
            doc.Id = ObjectId.GenerateNewId();
            doc.TableIndex = cenas.tableIndex;
            doc.TableName = cenas.tableName;

            documentsHelper.CreateDocument(doc);
            var response = Request.CreateResponse<aux>(HttpStatusCode.Created, cenas);
            return response;
        }
    }
    public class aux
    {
        public string filePath { get; set; }
        public string tableName { get; set; }
        public string tableIndex { get; set; }
    }
}
