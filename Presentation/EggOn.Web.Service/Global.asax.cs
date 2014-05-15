using FlowOptions.EggOn.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace FlowOptions.EggOn.Service
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        public static List<Assembly> ModuleAssemblies; 

        protected void Application_Start()
        {
            Logger.Debug("Application is starting.");

            var config = GlobalConfiguration.Configuration;

            Logger.Debug("Application is loading modules.");
            var moduleAssemblies = LoadModuleAssemblies();
            WebApiApplication.ModuleAssemblies = moduleAssemblies;

            //AreaRegistration.RegisterAllAreas();
            DataConfig.Register(config, moduleAssemblies);
            MapperConfig.Register(config, moduleAssemblies);
            ModuleConfig.Register(config, moduleAssemblies);
            WebApiConfig.Register(config, moduleAssemblies);
            FilterConfig.Register(config, moduleAssemblies);

            GlobalConfiguration.Configuration.EnsureInitialized();

            Logger.Info("Application has started.");
        }

        private List<Assembly> LoadModuleAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();

            string modulePaths = ConfigurationManager.AppSettings.Get("Modules");

            if (String.IsNullOrWhiteSpace(modulePaths))
            {
                return assemblies;
            }

            foreach (string modulePath in modulePaths.Split(';'))
            {
                var path = modulePath.Trim();

                if (String.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                if (!Path.IsPathRooted(path))
                {
                    path = Path.GetFullPath(Path.Combine(HttpRuntime.AppDomainAppPath, path));
                }

                // TODO: Directories/wildcards.

                assemblies.Add(Assembly.LoadFrom(path));
            }

            return assemblies;
        }
    }
}