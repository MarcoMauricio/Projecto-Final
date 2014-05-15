using FlowOptions.EggOn.Logging;
using System;
using System.Web.Http;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using FlowOptions.EggOn.ModuleCore;

namespace FlowOptions.EggOn.Service
{
    public static class ModuleConfig
    {
        public static void Register(HttpConfiguration config, List<Assembly> moduleAssemblies)
        {
            Logger.Debug("Application is registering the modules.");

            // Load all other modules.
            var moduleInterface = typeof(IEggOnModule);

            List<Type> moduleTypes;
            
            try
            {
                moduleTypes = moduleAssemblies.SelectMany(s => s.GetTypes())
                              .Where(p => moduleInterface.IsAssignableFrom(p) && !p.IsInterface)
                              .ToList();
            }
            catch (ReflectionTypeLoadException e)
            {
                Logger.Fatal("Error while loading modules: " + e.LoaderExceptions[0]);
                return;
            }

            foreach (var moduleType in moduleTypes)
            {

                var module = (IEggOnModule)Activator.CreateInstance(moduleType);
                module.Setup();
            }
        }
    }
}
