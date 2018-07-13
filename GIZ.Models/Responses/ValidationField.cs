using System;
using System.Collections.Generic;
using System.Linq;

namespace GIZ.Models.Responses
{
    public class ValidationField
    {
        public string Property { get; set; }

        public string Message { get; set; }

        public string Value { get; set; }
    }
}