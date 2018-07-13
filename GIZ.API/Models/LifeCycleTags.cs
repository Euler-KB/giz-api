using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    [Table("LifeCycleTags")]
    public class LifeCycleTag : IIdentifiable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Index("IX_KeyLifeCylce", Order = 2, IsUnique = true)]
        public long LifeCycleId { get; set; }

        [ForeignKey("LifeCycleId")]
        public virtual ProductLifeCycle LifeCycle { get; set; }

        [Required]
        [StringLength(128)]
        [Index("IX_KeyLifeCylce" , Order = 1 , IsUnique = true)]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }
    }

}