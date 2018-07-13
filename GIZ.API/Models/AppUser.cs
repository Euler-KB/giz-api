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
    [Table("Users")]
    public class AppUser : IIdentifiable, ITimestamp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public AppUser()
        {
            Tokens = new HashSet<UserToken>();
            Products = new HashSet<Product>();
            Companies = new HashSet<CompanyInfo>();
        }

        /// <summary>
        /// Indicates whether the user account is active
        /// </summary>
        public bool IsActive { get; set; }

        [Required]
        [StringLength(256)]
        [Index("IX_UsernameUniq", IsUnique = true, Order = 1)]
        public string Username { get; set; }

        [Phone]
        [Index("IX_PhoneUniq", IsUnique = true, Order = 1)]
        [StringLength(128)]
        public string Phone { get; set; }

        public bool IsPhoneConfirmed { get; set; }

        [EmailAddress]
        [StringLength(256)]
        [Index("IX_EmailUniq", IsUnique = true, Order = 1)]
        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        public string PasswordSalt { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Specifies the user account type
        /// </summary>
        [Index("IX_PhoneUniq", IsUnique = true, Order = 2)]
        [Index("IX_EmailUniq", IsUnique = true, Order = 2)]
        public UserAccountType AccountType { get; set; }

        /// <summary>
        /// The profile image of the user
        /// </summary>
        public virtual Media ProfileImage { get; set; }

        [Required]
        public string FullName { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<UserToken> Tokens { get; set; }

        [InverseProperty("Dealer")]
        public virtual ICollection<Product> Products { get; set; }

        [InverseProperty("Dealer")]
        public virtual ICollection<CompanyInfo> Companies { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? LastAccessTime { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class AppUserConfig : EntityTypeConfiguration<AppUser>
    {
        public AppUserConfig()
        {
            HasMany(x => x.Tokens).WithRequired(x => x.User);
            HasMany(x => x.Products).WithRequired(x => x.Dealer);
            HasMany(x => x.Companies).WithRequired(x => x.Dealer);
            HasOptional(x => x.ProfileImage).WithOptionalPrincipal();
        }
    }
}