using AutoMapper;
using FlowOptions.EggOn.Base.Controllers;
using FlowOptions.EggOn.Data.Models;
using FlowOptions.EggOn.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace FlowOptions.EggOn.Data.Controllers
{
    public class DataFieldTypesController : EggOnApiController
    {
        [Route("data/fieldtypes"), HttpGet]
        public List<FieldTypeDto> GetAllDataFieldTypes()
        {
            var fieldTypes = Database.All<FieldType>();

            return Mapper.Map<List<FieldTypeDto>>(fieldTypes);
        }
    }
}
