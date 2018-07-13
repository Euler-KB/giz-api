using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    [Table("Companies")]
    public class CompanyInfo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual AppUser Dealer { get; set; }

        [InverseProperty("Company")]
        public virtual ICollection<CompanyBranch> Branches { get; set; }

        /// <summary>
        /// The region the company is based in
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// The country the company is based in
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// The location of the main branch
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Additional comment for the company's main branch
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The latitude component of the location
        /// </summary>
        public decimal? LocationLat { get; set; }

        /// <summary>
        /// The longitude component of the location
        /// </summary>
        public decimal? LocationLng { get; set; }

        /// <summary>
        /// The phone number of the company
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// The email of the company
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The date the company was established. The initial startup date
        /// </summary>
        public DateTime? EstablishmentDate { get; set; }

    }

    public class CompanyInfoConfig : EntityTypeConfiguration<CompanyInfo>
    {
        public CompanyInfoConfig()
        {
            HasMany(x => x.Branches).WithRequired(x => x.Company);
        }
    }
}