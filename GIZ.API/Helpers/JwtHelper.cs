using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GIZ.API.Helpers
{
    public static class JwtHelper
    {
        /// <summary>
        /// Generate token
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="role">The user's role</param>
        public static string GenerateToken(string userId, string role, string audience = null , int ? durationDays = null)
        {
            var handler = new JwtSecurityTokenHandler();
            string issuer = ConfigurationManager.AppSettings["JWT_ISSUER"];
            string secret = ConfigurationManager.AppSettings["JWT_SECRET"];
            audience = audience ?? ConfigurationManager.AppSettings["JWT_AUDIENCE"];
            int jwtDurationDays = durationDays ?? int.Parse(ConfigurationManager.AppSettings["JWT_DURATION_DAYS"]);

            return handler.WriteToken(handler.CreateJwtSecurityToken(issuer, audience, expires: DateTime.UtcNow.AddDays(jwtDurationDays),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                SecurityAlgorithms.HmacSha256)));
        }

        /// <summary>
        /// Validate token
        /// </summary>
        /// <param name="token">The token to validate</param>
        public static ClaimsPrincipal ValidateToken(string token, string audience = null , bool validateExpiration = true)
        {
            string issuer = ConfigurationManager.AppSettings["JWT_ISSUER"];
            string secret = ConfigurationManager.AppSettings["JWT_SECRET"];
            audience = audience ?? ConfigurationManager.AppSettings["JWT_AUDIENCE"];

            var handler = new JwtSecurityTokenHandler();

            try
            {
                SecurityToken _token;
                return handler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateLifetime = validateExpiration,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                }, out _token);
            }
            catch
            {
                return null;
            }
        }
    }
}