using System.Collections.Generic;
using System.Configuration;
using System.Net;
using AutoMapper;
using FlowOptions.EggOn.Base.Controllers;
using FlowOptions.EggOn.Context.ViewModels;
using System.Web.Http;
using System.Net.Http;
using FlowOptions.EggOn.Context.Models;
using FlowOptions.EggOn.ModuleCore;
using System.Web;

namespace EggOn.Context.Controllers
{
    public class DocumentController : EggOnApiController
    {
        [Route("Context"), HttpPost]
        public DocumentDto CreateDocument()
        {
            DocumentDto doc = new DocumentDto();
            doc.Text = "Hello";
            doc.SummarizedText = "Test";
            return doc;
        }
 
    }
}
