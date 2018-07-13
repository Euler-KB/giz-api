using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    public interface IIdentifiable
    {
        long Id { get; set; }
    }
}