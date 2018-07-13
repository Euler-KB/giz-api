using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    [Table("ProductLifeCycles")]
    public class ProductLifeCycle : IIdentifiable, ITimestamp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [InverseProperty("LifeCycle")]
        public virtual ICollection<LifeCycleTag> Tags { get; set; }

        public virtual Product Product { get; set; }

        public int Order { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public virtual ICollection<Media> Media { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class ProductLifeCycleConfig : EntityTypeConfiguration<ProductLifeCycle>
    {
        ProductLifeCycleConfig()
        {
            HasMany(x => x.Tags).WithRequired(x => x.LifeCycle).HasForeignKey(t => t.LifeCycleId).WillCascadeOnDelete();
            HasMany(x => x.Media);
        }
    }
}