using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    public class ProductView : IIdentifiable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public long Id { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public long? UserId { get; set; }

        public long ProductId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}