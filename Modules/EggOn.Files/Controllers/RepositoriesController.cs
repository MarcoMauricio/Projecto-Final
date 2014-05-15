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

namespace FlowOptions.EggOn.Files.Controllers
{
    public class RepositoriesController : EggOnApiController
    {
        [Route("repositories"), HttpGet]
        public List<RepositoryDto> GetAllRepositories()
        {
            var repositories = this.Database.All<Repository>();

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

            Repository repository = Mapper.Map<Repository>(data);
            repository.Id = GuidComb.NewGuid();
            this.Database.Insert(repository);

            string repositoryPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
            System.IO.Directory.CreateDirectory(repositoryPath);

            return Mapper.Map<RepositoryDto>(repository);
        }

        [Route("repositories/{repositoryId:guid}"), HttpGet]
        public RepositoryDto GetRepository(Guid repositoryId)
        {
            Repository repository = this.Database.SingleOrDefault<Repository>(repositoryId);

            if (repository == null)
            {
                throw NotFound("Repository not Found.");
            }

            string repositoryPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
            System.IO.Directory.CreateDirectory(repositoryPath);

            return Mapper.Map<RepositoryDto>(repository);
        }

        [Route("repositories/{repositoryId:guid}"), HttpPut]
        public RepositoryDto PutRepository(Guid repositoryId, RepositoryDto data)
        {
            Repository repository = this.Database.SingleOrDefault<Repository>(repositoryId);

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
            this.Database.Update(repository);

            string repositoryPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
            System.IO.Directory.CreateDirectory(repositoryPath);

            return Mapper.Map<RepositoryDto>(repository);
        }

        [Route("repositories/{repositoryId:guid}"), HttpDelete]
        public RepositoryDto DeleteRepository(Guid repositoryId)
        {
            Repository repository = this.Database.SingleOrDefault<Repository>(repositoryId);

            if (repository == null)
            {
                throw NotFound("Repository not Found.");
            }

            using (var tr = this.Database.GetTransaction())
            {
                this.Database.Delete<File>("WHERE RepositoryId = @0", repository.Id);
                this.Database.Delete(repository);

                tr.Complete();
            }

            // TODO: Think about deleting the repository or archive it.
            string repositoryPath = System.IO.Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["RepositoriesPath"], repository.Id.ToString());
            System.IO.Directory.Delete(repositoryPath, true);

            return Mapper.Map<RepositoryDto>(repository);
        }


        [Route("repositories/{repositoryId:guid}/files/"), HttpGet]
        public List<FileDto> GetRepositoryRoot(Guid repositoryId)
        {
            Repository repository = this.Database.SingleOrDefault<Repository>(repositoryId);

            if (repository == null)
            {
                throw NotFound("Repository not Found.");
            }

            var files = this.Database.Fetch<File>("WHERE RepositoryId = @0 AND ParentFileId IS NULL", repository.Id);

            return Mapper.Map<List<FileDto>>(files);
        }
    }
}
