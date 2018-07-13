using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class UserModel
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Phone { get; set; }

        public string FullName { get; set; }

        public UserAccountType AccountType { get; set; }

        public IList<DealerCompanyInfo> Companies { get; set; }

        /// <summary>
        /// The profile image of the user
        /// </summary>
        public virtual MediaModel ProfileImage { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class DealerCompanyInfo
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<CompanyBranchInfo> Branches { get; set; }

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

    public class CompanyBranchInfo
    {
        public long Id { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string Location { get; set; }

        public decimal? LocationLat { get; set; }

        public decimal? LocationLng { get; set; }

        public string Comment { get; set; }

        public string Phone { get; set; }

        public DateTime? EstablishmentDate { get; set; }

    }

    public class UpdateCompanyInfo
    {
        [Required]
        public long Id { get; set; }

        /// <summary>
        /// The branches to add
        /// </summary>
        public IList<RegisterBranchInfo> AddBranches { get; set; }

        /// <summary>
        /// The ids of the branches to remove
        /// </summary>
        public IList<long> RemoveBranchIds { get; set; }

        /// <summary>
        /// Updates the name of the company
        /// </summary>
        public string Name { get; set; }

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

    public class UpdateUserModel
    {
        /// <summary>
        /// The new companies to add
        /// </summary>
        public IList<RegisterCompanyInfo> AddCompany { get; set; }

        /// <summary>
        /// The info of the companies to update
        /// </summary>
        public IList<UpdateCompanyInfo> UpdateCompany { get; set; }

        /// <summary>
        /// The companies to remove
        /// </summary>
        public IList<long> RemoveCompanyIds { get; set; }

    }
}
