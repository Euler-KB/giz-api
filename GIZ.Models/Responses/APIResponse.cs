using System;
using System.Collections.Generic;
using System.Linq;

namespace GIZ.Models.Responses
{
    public enum ResponseStatus
    {
        /// <summary>
        /// Operation was successful
        /// </summary>
        Success = 0,

        /// <summary>
        /// Resource was not found
        /// </summary>
        NotFound = -4,

        /// <summary>
        /// A validation error occured
        /// </summary>
        ValidationError = -3,

        /// <summary>
        /// Operation failed
        /// </summary>
        Fail = -1,

        /// <summary>
        /// An internal server error occured
        /// </summary>
        InternalServerError = -6,

        /// <summary>
        /// User account not active
        /// </summary>
        UserAccountActivationRequired = -20
    }

    public class APIResponse
    {
        /// <summary>
        /// The status of the operation
        /// </summary>
        public ResponseStatus Status { get; set; }

        /// <summary>
        /// The associated message
        /// </summary>
        public string Message { get; set; }

    }

    public class APIDataResponse<T> : APIResponse
    {
        /// <summary>
        /// The data associated with response
        /// </summary>
        public T Data { get; set; }
    }

    public class APIValidationError : APIResponse
    {
        /// <summary>
        /// Holds the validation errors associated with the data
        /// </summary>
        public IEnumerable<ValidationField> ValidationErrors { get; set; }
    }
}