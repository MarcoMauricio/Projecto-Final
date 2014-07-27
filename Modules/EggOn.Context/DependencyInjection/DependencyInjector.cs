using System.Linq;
using EggOn.Context.NLP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Configuration;
using EggOn.Context.NLP.Services;
using FlowOptions.EggOn.Logging;

namespace EggOn.Context.DependencyInjection
{
    public class DependencyInjector
    {
        private ContextCore _contextCore;
        private string[] _servicesFiles;
        private List<IContextService> _servicesList;

        public DependencyInjector()
        {
            _servicesList = new List<IContextService>();
        }

        public ContextCore GetContextCore()
        {
            if (_contextCore != null) return _contextCore;
            var moduleInterface = typeof(IContextService);
            var moduleAssemblies = LoadServicesAssemblies();
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
                return null;
            }
            foreach (var module in moduleTypes.Select(moduleType => (IContextService)Activator.CreateInstance(moduleType)))
            {
                _servicesList.Add(module);
            }
            _contextCore = new ContextCore(_servicesList);
            return _contextCore;
        }

        private IEnumerable<Assembly> LoadServicesAssemblies()
        {
            var servicesPath = Path.Combine(HttpRuntime.AppDomainAppPath, ConfigurationManager.AppSettings["ContextServicesPath"]);
            try
            {
                _servicesFiles = Directory.GetFiles(servicesPath, "*dll", SearchOption.AllDirectories);
            }
            catch (Exception e)
            {
                Directory.CreateDirectory(servicesPath);
                _servicesFiles = Directory.GetFiles(servicesPath, "*dll", SearchOption.AllDirectories);
            }
            return _servicesFiles.Select(Assembly.LoadFrom).ToList();
        }
    }
}
