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
        private DocumentService documentsHelper;
        private CategoryService categoryHelper;
        private EntitiesService entitiesHelper;
        private SummaryService summaryHelper;
        public ContextController()
        {
            documentsHelper = new DocumentService();
            categoryHelper = new CategoryService();
            entitiesHelper = new EntitiesService();
            summaryHelper = new SummaryService();

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
            ContextCore context = new ContextCore(new LocalService(), cenas.filePath);
            MinedObject item = context.GetContext();
            // Document
            Document doc = new Document();
            doc.Id = ObjectId.GenerateNewId();
            doc.TableIndex = cenas.tableIndex;
            doc.TableName = cenas.tableName;
            doc.Summary = item.Summary.ToArray().ToString();
            doc.Category =  item.Category;

            documentsHelper.CreateDocument(doc);
            //Category
            /*
            Category cat = new Category();
            cat.Id = ObjectId.GenerateNewId();
            cat.CategoryName = item.Category;
            categoryHelper.AddCategory(doc.Id, cat);*/
            //Entities
            foreach (var entity in item.Entities)
            {
                Entity ent = new Entity();
                ent.Id = ObjectId.GenerateNewId();
                ent.entityName = entity;
                entitiesHelper.AddEntity(doc.Id, ent);
            }
            /*
            Summary sum = new Summary();
            sum.Id = ObjectId.GenerateNewId();
            sum.SummaryText = item.Summary.ToArray().ToString();
            summaryHelper.AddSummary(doc.Id, sum);*/


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
