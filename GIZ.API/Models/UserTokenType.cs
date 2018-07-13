using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GIZ.API.Models
{
    public enum UserTokenType
    {
        /// <summary>
        /// Activate account
        /// </summary>
        ActivateAccount,

        /// <summary>
        /// Reset account password
        /// </summary>
        ResetPassword
    }
}