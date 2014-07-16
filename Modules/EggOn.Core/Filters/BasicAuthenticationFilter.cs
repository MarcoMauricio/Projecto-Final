using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.Logging;
using FlowOptions.EggOn.ModuleCore.ViewModels;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace FlowOptions.EggOn.Base.Filters
{
    /// <summary>
    /// Generic Basic Authentication filter that checks for basic authentication
    /// headers and challenges for authentication if no authentication is provided
    /// Sets the Thread Principle with a GenericAuthenticationPrincipal.
    /// 
    /// You can override the OnAuthorize method for custom auth logic that
    /// might be application specific.    
    /// </summary>
    /// <remarks>Always remember that Basic Authentication passes username and passwords
    /// from client to server in plain text, so make sure SSL is used with basic auth
    /// to encode the Authorization header on all requests (not just the login).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BasicAuthenticationFilter : AuthorizationFilterAttribute
    {
        bool Active = true;

        public BasicAuthenticationFilter()
        { }

        /// <summary>
        /// Overriden constructor to allow explicit disabling of this
        /// filter's behavior. Pass false to disable (same as no filter
        /// but declarative)
        /// </summary>
        /// <param name="active"></param>
        public BasicAuthenticationFilter(bool active)
        {
            Active = active;
        }

        /// <summary>
        /// Override to Web API filter method to handle Basic Auth check
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var identity = ParseAuthorizationHeader(actionContext);
            if (identity == null)
            {
                if (Active && !SkipAuthorization(actionContext))
                    Challenge(actionContext);

                return;
            }


            if (!OnAuthorizeUser(identity.Name, identity.Password, actionContext))
            {
                if (Active && !SkipAuthorization(actionContext))
                    Challenge(actionContext);

                return;
            }

            var principal = new GenericPrincipal(identity, null);

            Thread.CurrentPrincipal = principal;

            // inside of ASP.NET this is required
            if (HttpContext.Current != null)
                HttpContext.Current.User = principal;

            base.OnAuthorization(actionContext);
        }

        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
        }

        /// <summary>
        /// Base implementation for user authentication.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected virtual bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            using (var database = new EggOnDatabase())
            {
                var user = database.SingleOrDefault<User>("WHERE Email = @0", username);

                if (user == null || !user.CheckPassword(password) || !user.Validated)
                {
                    Logger.Info("User authentication failed for '" + username + "' from " + GetClientIp(actionContext.Request) + ".");

                    return false;
                }

                user.LastAction = DateTime.Now;
                database.Update(user, new string[] { "LastAction" });

                HttpContext.Current.User = user;

                Logger.Trace("User was authenticated successfully: " + username + " (" + user.Name + ")");
            }


            return true;
        }

        /// <summary>
        /// Parses the Authorization header and creates user credentials
        /// </summary>
        /// <param name="actionContext"></param>
        protected virtual BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authHeader = null;
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null && auth.Scheme == "Basic")
                authHeader = auth.Parameter;

            if (string.IsNullOrEmpty(authHeader))
                return null;

            authHeader = Encoding.Default.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
                return null;

            return new BasicAuthenticationIdentity(tokens[0], tokens[1]);
        }


        /// <summary>
        /// Send the Authentication Challenge request
        /// </summary>
        /// <param name="message"></param>
        /// <param name="actionContext"></param>
        private void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;

            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new ServiceMessage()
            {
                Type = MessageType.Error,
                HttpStatusCode = HttpStatusCode.Unauthorized,
                Message = "You need to authenticate first before accessing this resource."
            });

            if (!actionContext.Request.Headers.Contains("Referer") &&
                (!actionContext.Request.Headers.Contains("X-Requested-With") || 
                !actionContext.Request.Headers.GetValues("X-Requested-With").Contains("XMLHttpRequest")))
            {
                actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", host));
            }
        }

        private string GetClientIp(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            // WCF Specific Code
            /*
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop;
                prop = (RemoteEndpointMessageProperty)this.Request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            */
            else
            {
                return null;
            }
        }

    }

    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password)
            : base(name, "Basic")
        {
            Password = password;
        }

        /// <summary>
        /// Basic Auth Password for custom authentication
        /// </summary>
        public string Password { get; set; }
    }
}
