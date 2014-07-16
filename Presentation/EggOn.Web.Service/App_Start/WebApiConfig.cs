using FlowOptions.EggOn.Logging;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Linq;
using System;
using System.Web.Http.ExceptionHandling;
using System.Net.Http;
using System.Net;
using FlowOptions.EggOn.ModuleCore.ViewModels;
using System.Web;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore.Utilities;
using System.Threading.Tasks;
using System.Threading;

namespace FlowOptions.EggOn.Service
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, List<Assembly> moduleAssemblies)
        {
            Logger.Debug("Application is registering the Global Web API.");

            config.EnableCors(new EnableCorsAttribute(
                "*",
                "Authorization, Content-Type, Accept, X-Requested-With", 
                "GET, POST, PUT, DELETE, OPTIONS"
            ));

            // Set our own assembly resolver where we add the assemblies we need
            var assemblyResolver = new CustomAssembliesResolver(moduleAssemblies);
            config.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);

            // Set our own exception handler
            var exceptionHandler = new CustomExceptionHandler();
            config.Services.Replace(typeof(IExceptionHandler), exceptionHandler);

            // Service Index.
            config.Routes.MapHttpRoute(
                name: "API Index",
                routeTemplate: "",
                defaults: new { controller = "Index", action = "Handle" }
            );

            // Map all routes.
            config.MapHttpAttributeRoutes();

            // Catch all for undefined method requests.
            config.Routes.MapHttpRoute(
                name: "API Not Found",
                routeTemplate: "{*url}",
                defaults: new { controller = "NotFound", action = "Handle" }
            );

            // Avoid problems with xml namespaces.
            //config.Formatters.XmlFormatter.UseXmlSerializer = true;

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }

    public class CustomAssembliesResolver : DefaultAssembliesResolver
    {
        private List<Assembly> moduleAssemblies;

        public CustomAssembliesResolver()
        {
            moduleAssemblies = new List<Assembly>();
        }

        public CustomAssembliesResolver(List<Assembly> assemblies)
        {
            if (assemblies == null)
                throw new ArgumentException("assemblies");

            moduleAssemblies = assemblies;
        }

        public override ICollection<Assembly> GetAssemblies()
        {
            //ICollection<Assembly> baseAssemblies = base.GetAssemblies();

            var assemblies = new List<Assembly>();

            assemblies.Add(Assembly.GetExecutingAssembly());

            assemblies.AddRange(moduleAssemblies);

            return assemblies;
        }
    }

    public class CustomExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var resp = context.Request.CreateResponse<ServiceMessage>(HttpStatusCode.InternalServerError, new ServiceMessage()
            {
                Type = MessageType.Error,
                HttpStatusCode = HttpStatusCode.InternalServerError,
                Message = ((!HttpContext.Current.IsDebuggingEnabled) ?
                    "Internal Server Error. Please contact administrator." :
                    context.Exception.Message)
            });

            context.Result = new ErrorResult(resp);

            //throw new HttpResponseException(resp);
        }

        internal class ErrorResult : IHttpActionResult
        {
            HttpResponseMessage _message;

            public ErrorResult(HttpResponseMessage message)
            {
                _message = message;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_message);
            }
        }
    }

    public class IndexController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Handle()
        {
            using (var database = new EggOnDatabase())
            {
                var user = database.SingleOrDefault<dynamic>("SELECT * FROM EggOn.CoreUsers WHERE Email = @0", User.Identity.Name);

                var loadedModules = WebApiApplication.ModuleAssemblies.Select(a => a.GetName().Name).ToArray();

                return Request.CreateResponse<ServiceIndex>(HttpStatusCode.NotFound, new ServiceIndex()
                {
                    Message = "Welcome to EggOn REST Service!",
                    Version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3),
                    CurrentUser = user.Name,
                    LoadedModules = loadedModules
                });
            }
        }
    }

    public class NotFoundController : ApiController
    {
        [HttpGet, HttpPut, HttpPost, HttpDelete, HttpOptions, HttpPatch, HttpHead]
        public HttpResponseMessage Handle(string url)
        {
            return Request.CreateResponse<ServiceMessage>(HttpStatusCode.NotFound, new ServiceMessage()
            {
                Type = MessageType.Error,
                HttpStatusCode = HttpStatusCode.NotFound,
                Message = "Resource '/" + url + "' was not found."
            });
        }
    }
}
