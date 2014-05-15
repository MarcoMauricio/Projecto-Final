using AutoMapper;
using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.Base.ViewModels;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.Logging;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;

namespace FlowOptions.EggOn.Base
{
    public class BaseModule : IEggOnModule 
    {
        public void Setup()
        {
            Logger.Debug("Application is registering the Core Area.");

            SetupDatabase();
        }

        private void SetupDatabase()
        {
            using (var database = new EggOnDatabase())
            {
                using (var tr = database.GetTransaction())
                {
                    if (database.FirstOrDefault<Language>("") == null)
                    {
                        database.Insert(new Language() { Id = GuidComb.NewGuid(), Name = "English", Code = "en" });
                        database.Insert(new Language() { Id = GuidComb.NewGuid(), Name = "Português", Code = "pt" });
                    }

                    if (database.FirstOrDefault<User>("") == null)
                    {
                        database.Insert(new User()
                        {
                            Id = GuidComb.NewGuid(),
                            Name = "Flow Options",
                            Email = "admin@flowoptions.com",
                            Password = "fo",
                            InterfaceLanguageId = database.ExecuteScalar<Guid>("SELECT Id FROM [EggOn].[CoreLanguages] WHERE Code = @0", "en"),
                            LastAction = DateTime.Now,
                            Validated = true
                        });
                    }

                    if (database.FirstOrDefault<Role>("") == null)
                    {
                        database.Insert(new Role()
                        {
                            Id = GuidComb.NewGuid(),
                            Name = "Administrator"
                        });
                    }

                    if (!database.TableExists("[EggOn].[CoreUsersRoles]"))
                    {
                        database.CreateOrUpdateTable("[EggOn].[CoreUsersRoles]", new List<SqlColumn>()
                        {
                            new SqlColumn() { Name = "UserId", Type = "uniqueidentifier", Unique = true, Constraint = new SqlConstraint("[EggOn].[CoreUsers]", "[Id]") },
                            new SqlColumn() { Name = "RoleId", Type = "uniqueidentifier", Unique = true, Constraint = new SqlConstraint("[EggOn].[CoreRoles]", "[Id]") }                            
                        });

                        database.Insert("[EggOn].[CoreUsersRoles]", null, false, new
                        {
                            UserId = database.ExecuteScalar<Guid>("SELECT Id FROM [EggOn].[CoreUsers] WHERE Email = @0", "admin@flowoptions.com"),
                            RoleId = database.ExecuteScalar<Guid>("SELECT Id FROM [EggOn].[CoreRoles] WHERE Name = @0", "Administrator")
                        });
                    }   

                    tr.Complete();
                }
            }
        }        
    }

    public class BaseModuleMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "BaseModuleMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<User, UserDto>().ReverseMap();
            Mapper.CreateMap<Role, RoleDto>().ReverseMap();
            Mapper.CreateMap<Layout, LayoutDto>().ReverseMap();
            Mapper.CreateMap<Language, LanguageDto>().ReverseMap();
        }
    }
}