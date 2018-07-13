using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    [Table("ProductProperties")]
    public class ProductProperty
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Index("IX_ProductPropUniq", Order = 2, IsUnique = true)]
        public long ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Required]
        [StringLength(128)]
        [Index("IX_ProductPropUniq", Order = 1 , IsUnique = true)]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}