using System;
using System.Collections.Generic;
using System.Linq;

namespace GIZ.Models.Responses
{
    public class PaginatedData<T>
    {
        /// <summary>
        /// The total items per page
        /// </summary>
        public long TotalItems { get; set; }

        /// <summary>
        /// The actual data
        /// </summary>
        public T Data { get; set; }

    }
}