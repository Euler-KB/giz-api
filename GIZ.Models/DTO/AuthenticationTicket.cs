using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class AuthenticationTicket
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime IssuedAt { get; set; }

        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// The id of the user
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// The type of account for the user
        /// </summary>
        public UserAccountType UserAccountType { get; set; }

        public long Duration { get; set; }
    }
}
