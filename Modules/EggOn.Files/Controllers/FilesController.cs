using System;
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
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using RestSharp;
using File = FlowOptions.EggOn.Files.Models.File;

namespace FlowOptions.EggOn.Files.Controllers
{
    public class FilesController : EggOnApiController
    {
        private const string Username = "admin@flowoptions.com";
        private const string Password = "fo";

        [Route("files"), HttpPost]
        public FileDto CreateFile()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                var data = Request.Content.ReadAsAsync<FileDto>().Result;

                if (data == null || data.Type == 1)
                {
                    throw BadRequest("Request must be multipart.");
                }

                var repository = Database.SingleOrDefault<Repository>(data.RepositoryId);
                if (repository == null)
                {
                    throw NotFound("Repository not found.");
                }

                var file = Mapper.Map<File>(data);
                file.Id = GuidComb.NewGuid();
                file.Size = 0;
                file.Type = FileTypes.Folder;
                file.UploadDate = DateTime.Now;
                file.ContentType = null;
                file.Contents = new byte[0];
                file.UserId = CurrentUser.Id;
                file.RepositoryId = data.RepositoryId;
                file.ParentFileId = data.ParentFileId;

                var repositoryPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
                var filePath = Path.Combine(repositoryPath, GenerateFilePath(file));

                if (Directory.Exists(filePath) || System.IO.File.Exists(filePath))
                {
                    throw BadRequest("A file or folder already exists in that location.");
                }

                Directory.CreateDirectory(filePath);

                Database.Insert(file);

                return Mapper.Map<FileDto>(file);
            }
            else
            {
                if (HttpContext.Current.Request.Files.Count == 0)
                {
                    throw BadRequest("No file was uploaded.");
                }

                var uploadedFile = HttpContext.Current.Request.Files[0];

                var size = uploadedFile.ContentLength;

                byte[] contents = null;
                using (var binaryReader = new BinaryReader(uploadedFile.InputStream))
                {
                    contents = binaryReader.ReadBytes(size);
                }

                /// Ficheiro uploaded
                var file = new File()
                {
                    Id = GuidComb.NewGuid(),
                    Name = uploadedFile.FileName,
                    Size = size,
                    Type = FileTypes.File,
                    ContentType = uploadedFile.ContentType,
                    UserId = CurrentUser.Id,
                    UploadDate = DateTime.Now
                };




                if (Query.ContainsKey("repositoryId"))
                {
                    var repository = Database.SingleOrDefault<Repository>(Guid.Parse(Query["repositoryId"]));
                    if (repository == null)
                    {
                        throw NotFound("Repository not found.");
                    }

                    // File contents in the file system.
                    file.RepositoryId = Guid.Parse(Query["repositoryId"]);
                    file.ParentFileId = (Query.ContainsKey("parentFileId")) ? (Guid?)Guid.Parse(Query["parentFileId"]) : null;
                    file.Contents = new byte[0];

                    var repositoryPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
                    var path = "";

                    var parentFile = Database.SingleOrDefault<File>(file.ParentFileId);
                    while (parentFile != null)
                    {
                        if (parentFile.Type != FileTypes.Folder)
                        {
                            throw BadRequest("File is not a folder.");
                        }

                        path = Path.Combine("/" + parentFile.Name, path);
                        parentFile = Database.SingleOrDefault<File>(parentFile.ParentFileId);
                    }

                    Directory.CreateDirectory(Path.Combine(repositoryPath, path));

                    var filePath = Path.Combine(repositoryPath, path, file.Name);
                    System.IO.File.WriteAllBytes(filePath, contents);


                    if (file.Name.EndsWith(".pdf") || file.Name.EndsWith(".txt"))
                    {
                        var client = new RestClient("http://localhost:8075/");
                        var request = new RestRequest("context", Method.POST)
                        {
                            Credentials = new NetworkCredential(Username, Password)
                        };
                        request.AddParameter("FilePath", filePath);
                        request.AddParameter("TableName", "FilesFiles");
                        request.AddParameter("TableIndex", file.Id);
                        request.AddParameter("FileName", file.Name);
                        var response = client.Execute(request);
                        Console.WriteLine(response.Content);
                    }

                    else
                    {
                        // File contents embedded in the database.
                        file.Contents = contents;
                    }


                }
                Database.Insert(file);

                return Mapper.Map<FileDto>(file);
            }
        }

        [Route("files"), HttpGet]
        public List<FileDto> GetFiles(Guid parentFileId)
        {
            var files = Database.Fetch<File>("WHERE ParentFileId = @0", parentFileId);

            return Mapper.Map<List<FileDto>>(files);
        }

        [Route("files/{fileId:guid}")]
        public FileDto GetFile(Guid fileId)
        {
            var file = Database.SingleOrDefault<File>(fileId);

            if (file == null)
            {
                throw NotFound("File not found.");
            }

            return Mapper.Map<FileDto>(file);
        }

        [Route("files/{fileId:guid}"), HttpPut]
        public FileDto UpdateFile(Guid fileId, FileDto data)
        {
            var file = Database.SingleOrDefault<File>(fileId);

            if (file == null)
            {
                throw NotFound("File not found.");
            }

            file.Name = data.Name;
            file.ParentFileId = data.ParentFileId;

            Database.Update(file);

            return Mapper.Map<FileDto>(file);
        }

        [Route("files/{fileId:guid}"), HttpDelete]
        public FileDto DeleteFile(Guid fileId)
        {
            var file = Database.SingleOrDefault<File>(fileId);

            if (file == null)
            {
                throw NotFound("File not found.");
            }

            if (file.RepositoryId != null)
            {
                var repository = Database.SingleOrDefault<Repository>(file.RepositoryId);
                if (repository == null)
                {
                    throw NotFound("Repository not found.");
                }

                // TODO: Cascade delete.

                Database.Delete(file);

                var repositoryPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
                var filePath = Path.Combine(repositoryPath, GenerateFilePath(file));

                if (Directory.Exists(filePath))
                {
                    Directory.Delete(filePath, true);
                }
                else if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            else
            {
                Database.Delete(file);
            }

            return Mapper.Map<FileDto>(file);
        }

        [Route("download/{fileId:guid}"), HttpGet, AllowAnonymous]
        public HttpResponseMessage DownloadFile(Guid fileId, string filepath = "")
        {
            var file = Database.SingleOrDefault<File>(fileId);

            if (file == null)
            {
                throw NotFound("File not found.");
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);

            if (file.RepositoryId != null)
            {
                var repository = Database.SingleOrDefault<Repository>(file.RepositoryId);
                if (repository == null)
                {
                    throw NotFound("Repository not found.");
                }

                var repositoryPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
                var filePath = Path.Combine(repositoryPath, GenerateFilePath(file));

                // TODO: Use streams or async to prevent huge memory usage.
                response.Content = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
            }
            else
            {
                response.Content = new ByteArrayContent(file.Contents);
            }

            var contentType = file.ContentType;
            if (String.IsNullOrWhiteSpace(contentType))
            {
                contentType = MimeTypesHelper.GetMimeType(file.Name);
            }

            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            return response;
        }


        private string GenerateFilePath(File file)
        {
            var path = file.Name;

            var parentFile = Database.SingleOrDefault<File>(file.ParentFileId);
            while (parentFile != null)
            {
                path = Path.Combine("/" + parentFile.Name, path);
                parentFile = Database.SingleOrDefault<File>(parentFile.ParentFileId);
            }

            return path;
        }
    }
}
