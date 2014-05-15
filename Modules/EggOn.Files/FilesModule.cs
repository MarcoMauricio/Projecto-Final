using AutoMapper;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.Files.Models;
using FlowOptions.EggOn.Files.ViewModels;
using FlowOptions.EggOn.Logging;
using FlowOptions.EggOn.ModuleCore;

namespace EggOn.Web.Service.Areas.Files
{
    public class FilesModule : IEggOnModule
    {
        public void Setup()
        {
            Logger.Debug("Application is registering the Files Area.");

            SetupDatabase();
        }

        private void SetupDatabase()
        {
            using (var database = new EggOnDatabase())
            {
                using (var tr = database.GetTransaction())
                {
                    tr.Complete();
                }
            }
        }        
    }

    public class FilesModuleMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "FilesModuleMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<File, FileDto>().ReverseMap();
            Mapper.CreateMap<Repository, RepositoryDto>().ReverseMap();
        }
    }
}
