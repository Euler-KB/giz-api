using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    public interface ITimestamp
    {
        DateTime DateCreated { get; set; }

        DateTime LastUpdated { get; set; }
    }
}