using FlowOptions.EggOn.Logging;
using FlowOptions.EggOn.ModuleCore.ViewModels;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace FlowOptions.Web.Service.Filters
{
    public class ExceptionHandlerFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var message = "Exception was thrown when calling \"" + context.Request.RequestUri.AbsolutePath + "\": " + context.Exception.Message;

            Logger.Error(message);

            context.Response = context.Request.CreateResponse<ServiceMessage>(HttpStatusCode.InternalServerError, new ServiceMessage()
            {
                Type = MessageType.Error,
                HttpStatusCode = HttpStatusCode.InternalServerError,
                Message = ((!HttpContext.Current.IsDebuggingEnabled) ? 
                    "Internal Server Error. Please contact administrator." :
                    message)
            });
        }
    }
}