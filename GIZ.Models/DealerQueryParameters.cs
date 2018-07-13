using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models
{
    public class DealerQueryParameters
    {
        public string Name { get; set; }

        public string CompanyName { get; set; }

        public IList<string> Regions { get; set; }

        public IList<string> Countries { get; set; }

        public string OrderBy { get; set; }

        [OneOf("ASC", "DESC")]
        public string OrderMode { get; set; }

        public DateTime? DateEstablishedRangeStart { get; set; }

        public DateTime? DateEstablisheedRangeEnd { get; set; }

        public DateTime? DateCreatedRangeStart { get; set; }

        public DateTime? DateCreatedRangeEnd { get; set; }

        public long? CurrentPage { get; set; }

        public long? ItemsPerPage { get; set; }
    }
}
