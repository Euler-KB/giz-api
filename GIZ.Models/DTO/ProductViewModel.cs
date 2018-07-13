using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class ProductViewModel
    {
        public long TotalViews { get; set; }

        public IList<UserView> UserViews { get; set; }
    }

    public class UserView
    {
        public long ? UserId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
