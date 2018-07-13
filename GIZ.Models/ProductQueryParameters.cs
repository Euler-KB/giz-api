using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models
{
    public class ProductQueryParameters : IValidatableObject
    {
        public ProductClassification? Classification { get; set; }

        public string SearchKeyword { get; set; }

        public IList<string> Crops { get; set; }

        public IList<string> Brands { get; set; }

        public IList<string> Solutions { get; set; }

        public string LifeCycleName { get; set; }

        public string LifeCycleDescription { get; set; }

        public string ProductName { get; set; }

        public string ProductType { get; set; }

        public long[] DealerIds { get; set; }

        public long[] ProductIds { get; set; }

        public string DealerName { get; set; }

        public string DealerCompanyName { get; set; }

        public IList<string> DealerRegions { get; set; }

        public string DealerCompanyPhone { get; set; }

        public string DealerCountry { get; set; }

        public long? CurrentPage { get; set; }

        public long? ItemsPerPage { get; set; }

        public DateTime? DateRangeStart { get; set; }

        public DateTime? DateRangeEnd { get; set; }

        public string OrderBy { get; set; }

        [OneOf("ASC", "DESC")]
        public string OrderMode { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (CurrentPage != null && ItemsPerPage == null)
                yield return new ValidationResult($"Both '{nameof(CurrentPage)}' and '{nameof(ItemsPerPage)}' are required for the query!");

        }
    }
}
