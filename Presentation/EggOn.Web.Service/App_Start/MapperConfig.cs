using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Linq;

namespace FlowOptions.EggOn.Service
{
    public static class MapperConfig
    {
        public static void Register(HttpConfiguration config, List<Assembly> moduleAssemblies)
        {
            var assemblies = new List<Assembly>();

            var profileTypes = moduleAssemblies.SelectMany(s => s.GetTypes())
                                .Where(p => typeof(Profile).IsAssignableFrom(p))
                                .ToList();

            Mapper.Initialize(x =>
            {
                foreach (var profileType in profileTypes)
                {
                    x.AddProfile((Profile)Activator.CreateInstance(profileType));
                }
            });
        }
    }
}
