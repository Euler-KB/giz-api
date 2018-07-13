using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GIZ.API.Filters
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class NullableArgumentsAttribute : Attribute
    {
        public string [] Required { get; }

        public NullableArgumentsAttribute(params string[] required)
        {
            Required = required;
        }
    }
}