using System.Linq;
using EggOn.Context.DataAccessLayer.Services;
using EggOn.Context.Models;
using EggOn.Context.NLP;
using EggOn.Context.NLP.Services;
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
        private readonly DocumentService _documentsHelper;
        private readonly EntitiesService _entitiesHelper;

        public ContextController()
        {
            _documentsHelper = new DocumentService();

            _entitiesHelper = new EntitiesService();

        }
        [Route("context"), HttpGet]
        public List<Document> GetAllDocuments()
        {
            var cursor = _documentsHelper.GetDocuments();
            return cursor.ToList();
        }


        [Route("context"), HttpPost]
        public HttpResponseMessage CreateContext(Aux cenas)
        {
            var context = new ContextCore(new RemoteService(), cenas.FilePath);
            var item = context.GetContext();
            // Document
            var doc = new Document
            {
                Id = ObjectId.GenerateNewId(),
                TableIndex = cenas.TableIndex,
                TableName = cenas.TableName,
                Summary = item.Summary,
                Category = item.Category
            };

            _documentsHelper.CreateDocument(doc);

            foreach (var ent in item.Entities.Select(entity => new Entity {Id = ObjectId.GenerateNewId(), EntityName = entity}))
            {
                _entitiesHelper.AddEntity(doc.Id, ent);
            }

            var response = Request.CreateResponse(HttpStatusCode.Created, cenas);
            return response;
        }
    }
    public class Aux
    {
        public string FilePath { get; set; }
        public string TableName { get; set; }
        public string TableIndex { get; set; }

    }
}
