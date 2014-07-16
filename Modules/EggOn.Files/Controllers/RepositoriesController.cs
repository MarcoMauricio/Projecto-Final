using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using AutoMapper;
using FlowOptions.EggOn.Base.Controllers;
using FlowOptions.EggOn.Files.ViewModels;
using System.Web.Http;
using System.Net.Http;
using FlowOptions.EggOn.Files.Models;
using FlowOptions.EggOn.ModuleCore;
using System.Web;
using File = FlowOptions.EggOn.Files.Models.File;

namespace FlowOptions.EggOn.Files.Controllers
{
    public class RepositoriesController : EggOnApiController
    {
        [Route("repositories"), HttpGet]
        public List<RepositoryDto> GetAllRepositories()
        {
            var repositories = Database.All<Repository>();

            return Mapper.Map<List<RepositoryDto>>(repositories);
        }

        [Route("repositories"), HttpPost]
        public RepositoryDto CreateRepository(RepositoryDto data)
        {
            if (data.Name == null || data.Name.Trim() == "")
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The field \"Name\" is not valid."));
            }

            if (data.Type != 1)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.BadRequest, "The field \"Type\" is not valid."));
            }

            var repository = Mapper.Map<Repository>(data);
            repository.Id = GuidComb.NewGuid();
            Database.Insert(repository);

            var repositoryPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
            Directory.CreateDirectory(repositoryPath);

            return Mapper.Map<RepositoryDto>(repository);
        }

        [Route("repositories/{repositoryId:guid}"), HttpGet]
        public RepositoryDto GetRepository(Guid repositoryId)
        {
            var repository = Database.SingleOrDefault<Repository>(repositoryId);

            if (repository == null)
            {
                throw NotFound("Repository not Found.");
            }

            var repositoryPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
            Directory.CreateDirectory(repositoryPath);

            return Mapper.Map<RepositoryDto>(repository);
        }

        [Route("repositories/{repositoryId:guid}"), HttpPut]
        public RepositoryDto PutRepository(Guid repositoryId, RepositoryDto data)
        {
            var repository = Database.SingleOrDefault<Repository>(repositoryId);

            if (repository == null)
            {
                throw NotFound("Repository not Found.");
            }

            if (data.Name == null || data.Name.Trim() == "")
            {
                throw BadRequest("The field \"Name\" is not valid.");
            }

            if (data.Type != repository.Type)
            {
                throw BadRequest("The field \"Type\" cannot be changed.");
            }

            Mapper.Map<RepositoryDto, Repository>(data, repository);
            repository.Id = repositoryId;
            Database.Update(repository);

            var repositoryPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
            Directory.CreateDirectory(repositoryPath);

            return Mapper.Map<RepositoryDto>(repository);
        }

        [Route("repositories/{repositoryId:guid}"), HttpDelete]
        public RepositoryDto DeleteRepository(Guid repositoryId)
        {
            var repository = Database.SingleOrDefault<Repository>(repositoryId);

            if (repository == null)
            {
                throw NotFound("Repository not Found.");
            }

            using (var tr = Database.GetTransaction())
            {
                Database.Delete<File>("WHERE RepositoryId = @0", repository.Id);
                Database.Delete(repository);

                tr.Complete();
            }

            // TODO: Think about deleting the repository or archive it.
            var repositoryPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
            Directory.Delete(repositoryPath, true);

            return Mapper.Map<RepositoryDto>(repository);
        }


        [Route("repositories/{repositoryId:guid}/files/"), HttpGet]
        public List<FileDto> GetRepositoryRoot(Guid repositoryId)
        {
            var repository = Database.SingleOrDefault<Repository>(repositoryId);

            if (repository == null)
            {
                throw NotFound("Repository not Found.");
            }

            var files = Database.Fetch<File>("WHERE RepositoryId = @0 AND ParentFileId IS NULL", repository.Id);

            return Mapper.Map<List<FileDto>>(files);
        }
    }
}
