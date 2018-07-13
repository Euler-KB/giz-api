using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class RegionInfo
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Country { get; set; }

        public LocationInfo Coordinates { get; set; }
    }
}
