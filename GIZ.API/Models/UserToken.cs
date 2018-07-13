using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    [Table("UserTokens")]
    public class UserToken : IIdentifiable , ITimestamp
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [InverseProperty("Tokens")]
        public virtual AppUser User { get; set; }

        public UserTokenType Type { get; set; }

        [Required]
        public string Salt { get; set; }

        [Required]
        public string Token { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class UserTokenConfig : EntityTypeConfiguration<UserToken>
    {
        public UserTokenConfig()
        {
            HasRequired(x => x.User).WithMany(x => x.Tokens);
        }
    }
}