using GIZ.API.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GIZ.API.Controllers
{
    [RoutePrefix("api/admin")]
    [AuthorizeAdmin]
    public class AdminController : BaseController
    {
        //class ResetPasswordModel
        //{
        //    public string Password { get; set; }
        //}

        //[ActionName("DisableAccount")]
        //public Task<HttpResponseMessage> DisableUserAccount(long userId)
        //{

        //}

        //[ActionName("RemoveProduct")]
        //public Task<HttpResponseMessage> RemoveProduct(long productId)
        //{

        //}

        //[ActionName("ResetUserPassword")]
        //public Task<HttpResponseMessage> ResetUserPassword(long userId , [FromBody]ResetPasswordModel model)
        //{

        //}

        //[ActionName("RestoreProduct")]
        //public Task<HttpResponseMessage> RestoreProduct(long productId)
        //{

        //}

        //[ActionName("EnableAccount")]
        //public Task<HttpResponseMessage> EnableUserAccount(long userId)
        //{

        //}

    }
}