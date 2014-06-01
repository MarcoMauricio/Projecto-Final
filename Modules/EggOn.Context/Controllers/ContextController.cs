using Context.DataAccessLayer;
using Context.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace FlowOptions.EggOn.Context.Controllers
{
    public class ContextController : EggOnApiController
    {
        static DocumentService categoriesHelper = new DocumentService();
        [Route("document"), HttpGet]
        public List<Document> GetAllDocuments()
        {
            var cursor = categoriesHelper.GetCategories();
            return cursor.ToList<Category>();
        }
    }
}
