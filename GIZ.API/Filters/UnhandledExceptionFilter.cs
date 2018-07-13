using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace GIZ.API.Filters
{
    public class UnhandledExceptionFilter : IExceptionFilter
    {
        public bool AllowMultiple => false;

        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if (actionExecutedContext.Exception is HttpResponseException)
            {
                actionExecutedContext.Response = (actionExecutedContext.Exception as HttpResponseException).Response;
            }
            else
            {
                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(new GIZ.Models.Responses.APIResponse()
                {
                    Message = APIResources.STATUS_INTERNAL_SERVER_ERR_MSG,
                    Status = GIZ.Models.Responses.ResponseStatus.InternalServerError
                });
            }

            return Task.FromResult(0);
        }
    }
}