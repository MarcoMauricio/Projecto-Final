using FlowOptions.EggOn.Logging;
using System;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Filters;
using System.Collections.Generic;
using System.Reflection;
using System.Configuration;
using System.IO;
using System.Web;

namespace FlowOptions.EggOn.Service
{
    public class FilterConfig
    {
        public static void Register(HttpConfiguration config, List<Assembly> moduleAssemblies)
        {
            Logger.Debug("Application is registering the Global Filters.");

            var filterInterface = typeof(FilterAttribute);
            var filterTypes = moduleAssemblies.SelectMany(s => s.GetTypes())
                                .Where(p => filterInterface.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

            foreach (Type filterType in filterTypes)
            {
                var filter = (FilterAttribute)Activator.CreateInstance(filterType);
                config.Filters.Add(filter);
            }
        }
    }
}