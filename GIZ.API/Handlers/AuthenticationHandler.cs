using GIZ.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Z.EntityFramework.Plus;

namespace GIZ.API.Handlers
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private GIZ.API.Models.GIZContext dbContext = new Models.GIZContext();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var auth = request.Headers.Authorization;
            if (auth != null && string.Equals(auth.Scheme, "Bearer"))
            {
                var principal = JwtHelper.ValidateToken(auth.Parameter);
                if (principal != null)
                {
                    var id = long.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
                    await dbContext.Users.Where(x => x.Id == id).UpdateAsync(x => new Models.AppUser() { LastAccessTime = DateTime.UtcNow });

                    Thread.CurrentPrincipal = principal;

                    if (HttpContext.Current != null)
                        HttpContext.Current.User = principal;
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}