using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    public class CompanyBranch
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }
        
        public string Location { get; set; }

        public decimal? LocationLat { get; set; }

        public decimal? LocationLng { get; set; }

        public string Comment { get; set; }

        public string Phone { get; set; }

        public DateTime ? EstablishmentDate { get; set; }

        public virtual CompanyInfo Company { get; set; }
    }
}