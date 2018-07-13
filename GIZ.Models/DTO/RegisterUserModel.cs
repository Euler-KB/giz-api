using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class RegisterUserModel : IValidatableObject
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Phone { get; set; }

        public UserAccountType AccountType { get; set; }

        public string Email { get; set; }

        public IList<RegisterCompanyInfo> Companies { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AccountType != UserAccountType.Dealer && Companies?.Count > 0)
                yield return new ValidationResult("A non dealer account cannot posses companies!");
        }
    }

    public class RegisterBranchInfo
    {
        public string Region { get; set; }

        public string Country { get; set; }

        public DateTime? EstablishmentDate { get; set; }

        public string Phone { get; set; }

        public string Comment { get; set; }
    }

    public class RegisterCompanyInfo
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Region { get; set; }

        [Required]
        public string Country { get; set; }

        public string Comment { get; set; }

        public string Location { get; set; }

        public decimal? LocationLat { get; set; }

        public decimal? LocationLng { get; set; }

        public DateTime? EstablishmentDate { get; set; }

        [Required]
        public string Phone { get; set; }

        public string Email { get; set; }

        public IList<RegisterBranchInfo> Branches { get; set; }

    }
}
