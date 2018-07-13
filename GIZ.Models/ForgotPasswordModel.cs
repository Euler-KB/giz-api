using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        public string UserInput { get; set; }
    }
}
