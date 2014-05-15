using AutoMapper;
using FlowOptions.EggOn.Base;
using FlowOptions.EggOn.Data.Models;
using FlowOptions.EggOn.Data.ViewModels;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.Logging;
using FlowOptions.EggOn.ModuleCore;

namespace FlowOptions.EggOn.Data
{
    public class DataModule : IEggOnModule
    {
        public void Setup()
        {
            Logger.Debug("Application is registering the Data Area.");

            SetupDatabase();
        }

        private void SetupDatabase()
        {
            using (var database = new EggOnDatabase())
            {
                using (var tr = database.GetTransaction())
                {
                    if (!database.SchemaExists("Data"))
                    {
                        Logger.Debug("DATABASE: Creating Schema Data.");
                        database.Execute(@"Create SCHEMA Data");
                    }

                    if (database.ExecuteScalar<int>("SELECT COUNT(*) FROM EggOn.DataFieldTypes") == 0)
                    {
                        database.Insert(new FieldType() { Id = GuidComb.NewGuid(), Name = "Unique Id", SqlType = "uniqueidentifier", CanBePrimary = true });
                        database.Insert(new FieldType() { Id = GuidComb.NewGuid(), Name = "Number", SqlType = "int", CanBePrimary = true });
                        database.Insert(new FieldType() { Id = GuidComb.NewGuid(), Name = "Text Line", SqlType = "nvarchar(512)", CanBePrimary = true });
                        database.Insert(new FieldType() { Id = GuidComb.NewGuid(), Name = "Text Box", SqlType = "nvarchar(MAX)", CanBePrimary = false });
                        database.Insert(new FieldType() { Id = GuidComb.NewGuid(), Name = "Password", SqlType = "nvarchar(512)", CanBePrimary = false });
                        database.Insert(new FieldType() { Id = GuidComb.NewGuid(), Name = "Label", SqlType = "nvarchar(MAX)", CanBePrimary = false });
                        database.Insert(new FieldType() { Id = GuidComb.NewGuid(), Name = "File", SqlType = "uniqueidentifier", CanBePrimary = false });
                        database.Insert(new FieldType() { Id = GuidComb.NewGuid(), Name = "Date and Time", SqlType = "datetime", CanBePrimary = false });
                    }

                    tr.Complete();
                }
            }
        }
    }

    public class DataModuleMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DataModuleMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Container, ContainerDto>().ReverseMap();
            Mapper.CreateMap<Field, FieldDto>().ReverseMap();
            Mapper.CreateMap<FieldType, FieldTypeDto>().ReverseMap();
        }
    }
}
