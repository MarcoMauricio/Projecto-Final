using Context.DataAccessLayer;
using Context.Models;
using FlowOptions.EggOn.Base.Controllers;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Web.Http;

namespace FlowOptions.EggOn.Context.Controllers
{
    public class ContextController : EggOnApiController
    {
        static DocumentService documentsHelper = new DocumentService();
        [Route("context"), HttpGet]
        public MongoCursor<Document> GetAllDocuments()
        {
            var cursor = documentsHelper.GetDocuments();
            return cursor;
        }
    }
}
