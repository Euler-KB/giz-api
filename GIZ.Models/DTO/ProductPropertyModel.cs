using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class ProductPropertyModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

    }

    public class CreateProductProperty
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
