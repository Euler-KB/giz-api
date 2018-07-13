using GIZ.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Z.EntityFramework.Plus;

namespace GIZ.API
{
    public static class EntityFilterConfig
    {
        public static void Config()
        {
            QueryFilterManager.Filter<Product>(EntityFilterKeys.ActiveProducts, x => x.Where(t => t.DateDeleted == null));
            QueryFilterManager.Filter<AppUser>(EntityFilterKeys.ActiveUsers, x => x.Where(t => t.IsActive), false);
        }
    }
}