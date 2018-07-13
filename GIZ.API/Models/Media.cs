using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    [Table("Media")]
    public class Media : IIdentifiable, ITimestamp
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public long Id { get; set; }

        [Required]
        [StringLength(256)]
        public string OriginalPath { get; set; }

        [StringLength(256)]
        public string ThumbnailPath { get; set; }

        [Required]
        public string Tag { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

    }
}