using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class ProductSolutionModel
    {
        public long Id { get; set; }

        /// <summary>
        /// The solution statement
        /// </summary>
        public string Statement { get; set; }
    }

    public class CreateProductSolutionModel
    {
        [Required]
        public string Statement { get; set; }
    }
}
