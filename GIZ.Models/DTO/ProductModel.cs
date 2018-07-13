using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class ProductModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public string ProductType { get; set; }

        public ProductClassification Classification { get; set; }

        public virtual ICollection<ProductSolutionModel> Solutions { get; set; }

        public virtual ICollection<ProductLifeCycleModel> LifeCycle { get; set; }

        public IList<string> Brands { get; set; }

        public IList<string> Crops { get; set; }

        public IList<ProductPropertyModel> Properties { get; set; }

        /// <summary>
        /// The dealer of the product
        /// </summary>
        public ProductDealerInfo Dealer { get; set; }

        /// <summary>
        /// A photo or image of the product
        /// </summary>
        public MediaModel Image { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public ProductViewModel View { get; set; }

    }



    public class CreateProductModel
    {
        public long DealerId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Comment { get; set; }

        /// <summary>
        /// One of a the product types
        /// </summary>
        [Required]
        public string ProductType { get; set; }

        public IList<string> Brands { get; set; }

        public IList<string> Crops { get; set; }

        public ProductClassification Classification { get; set; }

        public IList<CreateLifeCycleModel> LifeCycles { get; set; }

        public IList<CreateProductSolutionModel> Solutions { get; set; }

        public IList<ProductPropertyModel> Properties { get; set; }

    }

    public class UpdateProductModel
    {
        /// <summary>
        /// Updates the name of the product
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Replaces all brands with the given list
        /// </summary>
        public IList<string> Brands { get; set; }

        /// <summary>
        /// Replaces all crops with the given list
        /// </summary>
        public IList<string> Crops { get; set; }

        /// <summary>
        /// Add new lifecycles for product
        /// </summary>
        public IList<CreateLifeCycleModel> AddLifeCycles { get; set; }

        /// <summary>
        /// Add new solutions the product tackles
        /// </summary>
        public IList<CreateProductSolutionModel> AddSolutions { get; set; }

        /// <summary>
        /// The list of properties to remove
        /// </summary>
        public IList<string> RemoveProperties { get; set; }

        /// <summary>
        /// A list of properties to add for product
        /// </summary>
        public IList<CreateProductProperty> AddProperties { get; set; }

        /// <summary>
        /// Remove lifecycles
        /// </summary>
        public IList<long> RemoveLifeCycleIds { get; set; }

        /// <summary>
        /// Remove solutions
        /// </summary>
        public IList<long> RemoveSolutionIds { get; set; }

        /// <summary>
        /// Update comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Updates the product's classification
        /// </summary>
        public ProductClassification? Classification { get; set; }

    }
}
