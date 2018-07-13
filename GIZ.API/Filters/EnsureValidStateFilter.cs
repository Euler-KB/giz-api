using GIZ.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;

namespace GIZ.API.Filters
{
    public class EnsureValidStateFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        HttpResponseMessage GetValidationResponse(HttpActionContext actionContext, IEnumerable<KeyValuePair<string, object>> inputs)
        {
            return actionContext.Request.CreateResponse(new APIValidationError()
            {
                Status = ResponseStatus.ValidationError,
                Message = APIResources.STATUS_VALIDATION_MSG,
                ValidationErrors = inputs.Select(x => new ValidationField()
                {
                    Message = "Parameter required!",
                    Property = x.Key,
                    Value = null
                })
            });
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateResponse(new APIValidationError()
                {
                    Status = ResponseStatus.ValidationError,
                    Message = APIResources.STATUS_VALIDATION_MSG,
                    ValidationErrors = actionContext.ModelState.Select(x =>
                   {
                       return new ValidationField()
                       {
                           Property = x.Key,
                           Message = x.Value.Errors.FirstOrDefault()?.ErrorMessage,
                           Value = x.Value?.Value?.RawValue?.ToString()
                       };
                   })
                });

                return;
            }

            //
            var nullableArgs = actionContext.ActionDescriptor.GetCustomAttributes<NullableArgumentsAttribute>();
            if (nullableArgs.Count > 0)
            {
                var nArgs = nullableArgs.First();
                var required = nArgs.Required;
                var invalidParams = actionContext.ActionArguments.Where(x => x.Value == null && required.Contains(x.Key));
                if (invalidParams.Count() > 0)
                    actionContext.Response = GetValidationResponse(actionContext, invalidParams);

            }
            else
            {
                var invalidParameters = actionContext.ActionArguments.Where(x => x.Value == null);
                if (invalidParameters.Count() > 0)
                    actionContext.Response = GetValidationResponse(actionContext, invalidParameters);
            }



        }
    }
}