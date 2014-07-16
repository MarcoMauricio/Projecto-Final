using AutoMapper;
using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.Base.ViewModels;
using System.Collections.Generic;
using System.Web.Http;

namespace FlowOptions.EggOn.Base.Controllers
{
    public class LanguagesController : EggOnApiController
    {
        [Route("languages"), HttpGet]
        public List<LanguageDto> GetAllLanguages()
        {
            var languages = Database.All<Language>();

            return Mapper.Map<List<LanguageDto>>(languages);
        }
    }
}
