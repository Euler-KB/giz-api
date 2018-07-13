using GIZ.API.Models;
using GIZ.Models.DTO;
using GIZ.Models.Responses;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace GIZ.API.Controllers
{
    [RoutePrefix("api/dealers")]
    public class DealersController : BaseController
    {
        [HttpGet]
        [Route("all")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<UserModel>>))]
        public async Task<HttpResponseMessage> GetAll()
        {
            return Data(Map<IEnumerable<UserModel>>(await DB.Users.Include(x => x.Companies)
                .Include(x => x.Companies.Select(t => t.Branches))
                .Include(x => x.ProfileImage)
                .AsNoTracking()
                .Where(x => x.AccountType == GIZ.Models.UserAccountType.Dealer)
                .ToListAsync()));
        }

        [HttpPost]
        [Route("query")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<UserModel>>))]
        public async Task<HttpResponseMessage> QueryDealers([FromBody]GIZ.Models.DealerQueryParameters query)
        {
            IQueryable<AppUser> filter = DB.Users.Include(x => x.Companies)
                .Include(x => x.Companies.Select(t => t.Branches))
                .Where(x => x.AccountType == GIZ.Models.UserAccountType.Dealer)
                .AsNoTracking();

            if (query.CompanyName != null)
                filter = filter.Where(x => x.Companies.Any(t => t.Name.Contains(query.CompanyName)));

            if (query.Countries?.Count >= 0)
                filter = filter.Where(x => query.Countries.Any(t => x.Companies.Any(k => k.Country.Equals(t)) || x.Companies.Any(f => f.Branches.Any(y => y.Country.Equals(t)))));

            if (query.Regions?.Count > 0)
                filter = filter.Where(x => query.Regions.Any(t => x.Companies.Any(k => k.Region.Equals(k)) || x.Companies.Any(f => f.Branches.Any(y => y.Region.Equals(y)))));

            if (query.Name != null)
                filter = filter.Where(x => x.FullName.Contains(query.Name));

            if (query.DateCreatedRangeEnd != null && query.DateCreatedRangeEnd != null)
                filter = filter.Where(x => x.DateCreated > query.DateCreatedRangeStart && x.DateCreated <= query.DateCreatedRangeEnd);
            else if (query.DateCreatedRangeEnd != null)
                filter = filter.Where(x => x.DateCreated <= query.DateCreatedRangeEnd);
            else if (query.DateCreatedRangeStart != null)
                filter = filter.Where(x => x.DateCreated > query.DateCreatedRangeStart);

            //
            if (query.DateEstablishedRangeStart != null && query.DateEstablisheedRangeEnd != null)
                filter = filter.Where(x => x.DateCreated > query.DateEstablishedRangeStart && x.DateCreated <= query.DateEstablisheedRangeEnd);
            else if (query.DateEstablisheedRangeEnd != null)
                filter = filter.Where(x => x.DateCreated <= query.DateEstablisheedRangeEnd);
            else if (query.DateEstablishedRangeStart != null)
                filter = filter.Where(x => x.DateCreated > query.DateEstablishedRangeStart);

            var order = query.OrderBy ?? "Id";
            var orderMode = query.OrderMode;
            switch (order)
            {
                case "Id":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.Id) : filter.OrderBy(x => x.Id);
                    break;
                case "Created":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.DateCreated) : filter.OrderBy(x => x.DateCreated);
                    break;
                case "TotalCompanies":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.Companies.Count) : filter.OrderBy(x => x.Companies.Count);
                    break;
                case "TotalBranches":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.Companies.Sum(t => t.Branches.Count)) : filter.OrderBy(x => x.Companies.Sum(t => t.Branches.Count));
                    break;
                case "Name":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.FullName) : filter.OrderBy(x => x.FullName);
                    break;
            }


            if (query.CurrentPage != null && query.ItemsPerPage != null)
            {
                //  Paginated response
                long totalItems = await filter.LongCountAsync();
                var data = await filter.Skip((int)(query.CurrentPage.Value * query.ItemsPerPage.Value)).Take((int)query.ItemsPerPage.Value).ToArrayAsync();
                return Pagination(Map<IEnumerable<UserModel>>(data), totalItems);
            }
            else
            {
                return Data(Map<IEnumerable<UserModel>>(await filter.ToArrayAsync()));
            }
        }

        [HttpGet]
        [Route("{id}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<UserModel>))]
        public async Task<HttpResponseMessage> GetDealerAsync(long id)
        {
            var user = await DB.Users.Where(x => x.IsActive && x.AccountType == GIZ.Models.UserAccountType.Dealer).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotExists();

            return Data(Map<UserModel>(user));
        }
    }
}