using AutoMapper;
using GIZ.API.Models;
using GIZ.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GIZ.API.Controllers
{
    public class BaseController : ApiController
    {
        private Models.GIZContext dbContext;

        /// <summary>
        /// Get the database context
        /// </summary>
        public Models.GIZContext DB
        {
            get
            {
                return dbContext ?? (dbContext = new GIZContext());
            }

            set
            {
                if (value != dbContext)
                    dbContext = value;
            }
        }

        /// <summary>
        /// Gets the current user
        /// </summary>
        protected AppUser CurrentUser => DB.Users.Find(UserId);

        [NonAction]
        protected Task<AppUser> GetCurrentUserAsync() => DB.Users.FindAsync(UserId);

        /// <summary>
        /// Gets the current user ID
        /// </summary>
        protected long UserId => long.Parse((User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value);

        /// <summary>
        /// Maps DTO
        /// </summary>
        protected T Map<T>(object item)
        {
            return Mapper.Instance.Map<T>(item);
        }

        /// <summary>
        /// Ships a data response to client
        /// </summary>
        /// <typeparam name="T">The type of the data to send</typeparam>
        /// <param name="data">The data to send to client</param>
        [NonAction]
        protected HttpResponseMessage Data<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return ApiResponse(new APIDataResponse<T>()
            {
                Data = data,
                Message = APIResources.STATUS_OK_MSG,
                Status = ResponseStatus.Success
            }, statusCode);
        }

        /// <summary>
        /// Returns paginated data
        /// </summary>
        [NonAction]
        protected HttpResponseMessage Pagination<T>(IEnumerable<T> items, long totalItems)
        {
            return Data(new PaginatedData<IEnumerable<T>>()
            {
                Data = items,
                TotalItems = totalItems
            });
        }

        /// <summary>
        /// Resources does not exist
        /// </summary>
        [NonAction]
        protected HttpResponseMessage NotExists(string message = null)
        {
            return ApiResponse(new APIResponse()
            {
                Status = ResponseStatus.NotFound,
                Message = message ?? APIResources.STATUS_NOT_FOUND_MSG
            }, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Returns validation errors
        /// </summary>
        [NonAction]
        protected HttpResponseMessage ValidationError(params ValidationField[] errors)
        {
            return ApiResponse(new APIValidationError()
            {
                Message = APIResources.STATUS_VALIDATION_MSG,
                Status = ResponseStatus.ValidationError,
                ValidationErrors = errors
            }, HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Returns an API Response
        /// </summary>
        [NonAction]
        protected HttpResponseMessage ApiResponse(APIResponse response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return Request.CreateResponse(statusCode, response);
        }

        /// <summary>
        /// Successful Operation
        /// </summary>
        [NonAction]
        protected HttpResponseMessage OperationSuccess(string statusMessage = "Operation completed successfully", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return ApiResponse(new APIResponse()
            {
                Status = ResponseStatus.Success,
                Message = statusMessage
            }, statusCode);
        }

        /// <summary>
        /// Failed operation
        /// </summary>
        [NonAction]
        protected HttpResponseMessage OperationFail(string statusMessage = "Operation failed", HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return ApiResponse(new APIResponse()
            {
                Status = ResponseStatus.Fail,
                Message = statusMessage
            }, statusCode);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext?.Dispose();
            }

            base.Dispose(disposing);
        }

    }
}