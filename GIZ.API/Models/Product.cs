using GIZ.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    [Table("Products")]
    public class Product : IIdentifiable, ITimestamp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Product()
        {
            Solutions = new List<ProductSolution>();
            LifeCycle = new List<ProductLifeCycle>();
            Media = new List<Media>();
            Properties = new List<ProductProperty>();
            Views = new List<ProductView>();
        }

        [Required]
        public string ProductType { get; set; }

        public ProductClassification Classification { get; set; }

        [Required]
        public string Name { get; set; }

        public string Comments { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<ProductSolution> Solutions { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<ProductLifeCycle> LifeCycle { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<ProductProperty> Properties { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<ProductView> Views { get; set; }

        public string Brand { get; set; }

        public string CropType { get; set; }

        /// <summary>
        /// The dealer who the product belongs to..
        /// </summary>
        public virtual AppUser Dealer { get; set; }

        [ForeignKey("Dealer")]
        public long DealerId { get; set; }

        /// <summary>
        /// A photo or image of the product
        /// </summary>
        public virtual ICollection<Media> Media { get; set; }

        public DateTime? DateDeleted { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class ProductEntityConfig : EntityTypeConfiguration<Product>
    {
        public ProductEntityConfig()
        {
            HasMany(x => x.Solutions).WithRequired(x => x.Product);
            HasMany(x => x.Properties).WithRequired(x => x.Product);
            HasMany(x => x.LifeCycle).WithRequired(x => x.Product);
            HasMany(x => x.Media).WithOptional();
        }
    }
}