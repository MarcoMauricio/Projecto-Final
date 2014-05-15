﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using AutoMapper;
using FlowOptions.EggOn.Base.Controllers;
using FlowOptions.EggOn.Files.ViewModels;
using System.Web.Http;
using System.Net.Http;
using FlowOptions.EggOn.Files.Models;
using FlowOptions.EggOn.ModuleCore;
using System.Web;

namespace FlowOptions.EggOn.Files.Controllers
{
    public class FilesController : EggOnApiController
    {
        [Route("files"), HttpPost]
        public FileDto CreateFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                FileDto data = Request.Content.ReadAsAsync<FileDto>().Result;

                if (data == null || data.Type == 1)
                {
                    throw BadRequest("Request must be multipart.");
                }

                var repository = this.Database.SingleOrDefault<Repository>(data.RepositoryId);
                if (repository == null)
                {
                    throw NotFound("Repository not found.");
                }

                File file = Mapper.Map<File>(data);
                file.Id = GuidComb.NewGuid();
                file.Size = 0;
                file.Type = FileTypes.Folder;
                file.UploadDate = DateTime.Now;
                file.ContentType = null;
                file.Contents = new byte[0];
                file.UserId = this.CurrentUser.Id;
                file.RepositoryId = data.RepositoryId;
                file.ParentFileId = data.ParentFileId;

                string repositoryPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
                var filePath = System.IO.Path.Combine(repositoryPath, GenerateFilePath(file));

                if (System.IO.Directory.Exists(filePath) || System.IO.File.Exists(filePath))
                {
                    throw BadRequest("A file or folder already exists in that location.");
                }

                System.IO.Directory.CreateDirectory(filePath);

                Database.Insert(file);

                return Mapper.Map<FileDto>(file);
            }
            else
            {
                if (HttpContext.Current.Request.Files.Count == 0)
                {
                    throw BadRequest("No file was uploaded.");
                }

                var uploadedFile = System.Web.HttpContext.Current.Request.Files[0];

                int size = uploadedFile.ContentLength;

                byte[] contents = null;
                using (var binaryReader = new System.IO.BinaryReader(uploadedFile.InputStream))
                {
                    contents = binaryReader.ReadBytes(size);
                }

                File file = new File()
                {
                    Id = GuidComb.NewGuid(),
                    Name = uploadedFile.FileName,
                    Size = size,
                    Type = FileTypes.File,
                    ContentType = uploadedFile.ContentType,
                    UserId = this.CurrentUser.Id,
                    UploadDate = DateTime.Now
                };

                if (Query.ContainsKey("repositoryId")) 
                {
                    var repository = Database.SingleOrDefault<Repository>(Guid.Parse(this.Query["repositoryId"]));
                    if (repository == null)
                    {
                        throw NotFound("Repository not found.");
                    }

                    // File contents in the file system.
                    file.RepositoryId = Guid.Parse(this.Query["repositoryId"]);
                    file.ParentFileId = (this.Query.ContainsKey("parentFileId")) ? (Guid?)Guid.Parse(this.Query["parentFileId"]) : null;
                    file.Contents = new byte[0];

                    string repositoryPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
                    string path = "";

                    File parentFile = this.Database.SingleOrDefault<File>(file.ParentFileId);
                    while(parentFile != null) {
                        if (parentFile.Type != FileTypes.Folder)
                        {
                            throw BadRequest("File is not a folder.");
                        }

                        path = System.IO.Path.Combine("/" + parentFile.Name, path);
                        parentFile = this.Database.SingleOrDefault<File>(parentFile.ParentFileId);
                    }

                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(repositoryPath, path));

                    string filePath = System.IO.Path.Combine(repositoryPath, path, file.Name);
                    System.IO.File.WriteAllBytes(filePath, contents);
                } 
                else 
                {
                    // File contents embedded in the database.
                    file.Contents = contents;
                }

                this.Database.Insert(file);

                return Mapper.Map<FileDto>(file);
            }
        }

        [Route("files"), HttpGet]
        public List<FileDto> GetFiles(Guid parentFileId)
        {
            var files = this.Database.Fetch<File>("WHERE ParentFileId = @0", parentFileId);

            return Mapper.Map<List<FileDto>>(files);
        }

        [Route("files/{fileId:guid}")]
        public FileDto GetFile(Guid fileId)
        {
            File file = this.Database.SingleOrDefault<File>(fileId);

            if (file == null)
            {
                throw NotFound("File not found.");
            }

            return Mapper.Map<FileDto>(file);
        }

        [Route("files/{fileId:guid}"), HttpPut]
        public FileDto UpdateFile(Guid fileId, FileDto data)
        {
            File file = this.Database.SingleOrDefault<File>(fileId);

            if (file == null)
            {
                throw NotFound("File not found.");
            }

            file.Name = data.Name;
            file.ParentFileId = data.ParentFileId;

            this.Database.Update(file);

            return Mapper.Map<FileDto>(file);
        }

        [Route("files/{fileId:guid}"), HttpDelete]
        public FileDto DeleteFile(Guid fileId)
        {
            File file = this.Database.SingleOrDefault<File>(fileId);

            if (file == null)
            {
                throw NotFound("File not found.");
            }

            if (file.RepositoryId != null)
            {
                var repository = this.Database.SingleOrDefault<Repository>(file.RepositoryId);
                if (repository == null)
                {
                    throw NotFound("Repository not found.");
                }

                // TODO: Cascade delete.

                this.Database.Delete(file);

                string repositoryPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
                var filePath = System.IO.Path.Combine(repositoryPath, GenerateFilePath(file));

                if (System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.Delete(filePath, true);
                }
                else if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            else
            {
                this.Database.Delete(file);
            }
            
            return Mapper.Map<FileDto>(file);
        }

        [Route("download/{fileId:guid}"), HttpGet, AllowAnonymous]
        public HttpResponseMessage DownloadFile(Guid fileId, string filepath = "")
        {
            File file = this.Database.SingleOrDefault<File>(fileId);

            if (file == null)
            {
                throw NotFound("File not found.");
            }

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            if (file.RepositoryId != null)
            {
                var repository = this.Database.SingleOrDefault<Repository>(file.RepositoryId);
                if (repository == null)
                {
                    throw NotFound("Repository not found.");
                }

                string repositoryPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
                var filePath = System.IO.Path.Combine(repositoryPath, GenerateFilePath(file));

                // TODO: Use streams or async to prevent huge memory usage.
                response.Content = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
            }
            else
            {
                response.Content = new ByteArrayContent(file.Contents);
            }

            string contentType = file.ContentType;
            if (String.IsNullOrWhiteSpace(contentType))
            {
                contentType = MimeTypesHelper.GetMimeType(file.Name);
            }

            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);

            return response;
        }


        private string GenerateFilePath(File file)
        {
            string path = file.Name;

            File parentFile = this.Database.SingleOrDefault<File>(file.ParentFileId);
            while (parentFile != null)
            {
                path = System.IO.Path.Combine("/" + parentFile.Name, path);
                parentFile = this.Database.SingleOrDefault<File>(parentFile.ParentFileId);
            }

            return path;
        }
    }
}
