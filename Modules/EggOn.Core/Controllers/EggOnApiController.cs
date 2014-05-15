using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlowOptions.EggOn.Base.Controllers
{
    public class EggOnApiController : ApiController
    {
        private EggOnDatabase databaseCache;
        private User currentUserCache;
        private Dictionary<string, string> queryCache;


        /// <summary>
        /// Gets a connection to the database.
        /// </summary>
        public EggOnDatabase Database
        {
            get
            {
                if (databaseCache == null)
                {
                    databaseCache = new EggOnDatabase();
                }

                return databaseCache;
            }
        }

        /// <summary>
        /// Gets the current user that made this request.
        /// </summary>
        public User CurrentUser
        {
            get
            {
                if (currentUserCache == null)
                {
                    if (this.User != null && this.User.Identity.IsAuthenticated)
                    {
                        currentUserCache = this.Database.SingleOrDefault<User>("WHERE Email = @0", this.User.Identity.Name);
                    }
                }

                return currentUserCache;
            }

            set
            {
                currentUserCache = value;
            }
        }

        /// <summary>
        /// Gets the HTTP querystring used in the request as a dictionary.
        /// </summary>
        public Dictionary<string, string> Query
        {
            get
            {
                if (queryCache == null)
                {
                    queryCache = Request.GetQueryNameValuePairs().ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
                }

                return queryCache;
            }
        }


        /// <summary>
        /// creates an <see cref="HttpResponseException"/> with a response code of 400
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">Explanation text for the client.</param>
        /// <returns>A new HttpResponseException</returns>
        protected new HttpResponseException BadRequest(string reason)
        {
            return CreateHttpResponseException(reason, HttpStatusCode.BadRequest);

        }

        /// <summary>
        /// creates an <see cref="HttpResponseException"/> with a response code of 404
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">Explanation text for the client.</param>
        /// <returns>A new HttpResponseException</returns>
        protected HttpResponseException NotFound(string reason)
        {
            return CreateHttpResponseException(reason, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// creates an <see cref="HttpResponseException"/> with a response code of 403
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">Explanation text for the client.</param>
        /// <returns>A new HttpResponseException</returns>
        protected HttpResponseException Forbidden(string reason)
        {
            return CreateHttpResponseException(reason, HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// Creates an <see cref="HttpResponseException"/> to be thrown by the api.
        /// </summary>
        /// <param name="reason">Explanation text, also added to the body.</param>
        /// <param name="code">The HTTP status code.</param>
        /// <returns>A new <see cref="HttpResponseException"/></returns>
        private HttpResponseException CreateHttpResponseException(string reason, HttpStatusCode code)
        {
            throw new HttpResponseException(this.Request.CreateResponse<ServiceMessage>(code, new ServiceMessage()
            {
                Type = MessageType.Error,
                HttpStatusCode = code,
                Message = reason
            }));
        }
    }
}