using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class ProductDealerInfo
    {
        public long Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// The profile image of the user
        /// </summary>
        public virtual MediaModel ProfileImage { get; set; }
    }

    public class ProductItemSummary
    {
        /// <summary>
        /// The type of the product
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// The dealers of the product
        /// </summary>
        public IList<ProductDealerInfo> Dealers { get; set; }

        /// <summary>
        /// The total number of items of registered
        /// </summary>
        public long TotalCount { get; set; }
    }
}
