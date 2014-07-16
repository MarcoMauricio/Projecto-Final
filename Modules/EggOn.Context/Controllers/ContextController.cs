﻿using System;
using System.Linq;
using EggOn.Context.DataAccessLayer.Services;
using EggOn.Context.Models;
using EggOn.Context.NLP;
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
        private static List<string> SupportedTypes = new List<string> { "txt", "pdf" };

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
        public HttpResponseMessage CreateContext(Message message)
        {
            if (!checkSupport(message.FilePath)) return Request.CreateResponse(HttpStatusCode.NotAcceptable, message);
            var item = ContextCore.GetContext(message.FilePath);
            // Document
            var doc = new Document
            {
                Id = ObjectId.GenerateNewId(),
                TableIndex = message.TableIndex,
                TableName = message.TableName,
                FileName = message.FileName,
                Summary = item.Summary,
                Category = item.Category,
                DateTime = new BsonDateTime(System.DateTime.Now)
            };

            _documentsHelper.CreateDocument(doc);

            foreach (var ent in item.Entities.Select(entity => new Entity { Id = ObjectId.GenerateNewId(), EntityName = entity }))
            {
                _entitiesHelper.AddEntity(doc.Id, ent);
            }

            return Request.CreateResponse(HttpStatusCode.Created, message); 
        }


        [Route("context"), HttpDelete]
        public void DeleteDocumentByFileName(string fileName)
        {
            var document = _documentsHelper.GetDocumentByFileName(fileName);
            if (document == null)
            {
                throw NotFound("Document not found.");
            }
            _documentsHelper.Delete(document.Id);
        }



        private bool checkSupport(string path)
        {
            foreach (var item in SupportedTypes)
            {
                if (path.EndsWith(item)) return true;
            }
            return false;
        }
    }
    public class Message
    {
        public string FilePath { get; set; }
        public string TableName { get; set; }
        public string TableIndex { get; set; }
        public string FileName { get; set; }
    }
}
