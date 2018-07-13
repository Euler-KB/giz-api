using GIZ.API.Filters;
using GIZ.API.Helpers;
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
using Z.EntityFramework.Plus;

namespace GIZ.API.Controllers
{
    [RoutePrefix("api/products")]
    public class ProductsController : BaseController
    {

        [Route("{id}/view")]
        [HttpPost]
        public async Task ViewProduct(long id)
        {
            if (!await DB.Products.AnyAsync(x => x.Id == id))
                throw new HttpResponseException(HttpStatusCode.Forbidden);

            if (User == null)
            {
                DB.ProductViews.Add(new ProductView()
                {
                    ProductId = id,
                    Timestamp = DateTime.UtcNow
                });
            }
            else
            {
                if (!await DB.ProductViews.AnyAsync(x => x.User.Id == UserId && x.Product.Id == id))
                {
                    DB.ProductViews.Add(new ProductView()
                    {
                        UserId = UserId,
                        ProductId = id,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }

            await DB.SaveChangesAsync();
        }

        [Route("all")]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<ProductModel>>))]
        public async Task<HttpResponseMessage> GetAllProducts()
        {
            return Data(Map<IEnumerable<ProductModel>>(await DB.Products.AsNoTracking()
                .Include(x => x.Media)
                .Include(x => x.Properties)
                .Include(x => x.Solutions)
                .Include(x => x.LifeCycle)
                .Include(x => x.Dealer)
                .Include(x => x.Views)
                .ToListAsync()));
        }

        [Route("query")]
        [HttpPost]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<ProductModel>>))]
        public async Task<HttpResponseMessage> QueryProducts([FromBody]GIZ.Models.ProductQueryParameters queryParameters)
        {
            IQueryable<Product> filter = DB.Products.AsNoTracking()
                .Include(x => x.Media)
                .Include(x => x.Properties)
                .Include(x => x.Solutions)
                .Include(x => x.Dealer)
                .Include(x => x.Views)
                .Include(x => x.LifeCycle);

            if (queryParameters.ProductIds != null)
                filter = filter.Where(x => queryParameters.ProductIds.Contains(x.Id));

            if (queryParameters.ProductName != null)
                filter = filter.Where(x => x.Name.Contains(queryParameters.ProductName));

            if (queryParameters.ProductType != null)
                filter = filter.Where(x => x.ProductType == queryParameters.ProductType);

            if (queryParameters.SearchKeyword != null)
            {
                filter = filter.Where(x => x.Name.Contains(queryParameters.SearchKeyword) || x.ProductType.Contains(queryParameters.SearchKeyword) ||
                 x.Brand.Contains(queryParameters.SearchKeyword) || x.Comments.Contains(queryParameters.SearchKeyword));
            }

            if (queryParameters.DateRangeEnd != null && queryParameters.DateRangeStart != null)
                filter = filter.Where(x => x.DateCreated > queryParameters.DateRangeStart && x.DateCreated <= queryParameters.DateRangeEnd);
            else if (queryParameters.DateRangeStart != null)
                filter = filter.Where(x => x.DateCreated > queryParameters.DateRangeStart);
            else if (queryParameters.DateRangeEnd != null)
                filter = filter.Where(x => x.DateCreated <= queryParameters.DateRangeEnd);

            if (queryParameters.Crops != null && queryParameters.Crops.Count > 0)
            {
                var crops = queryParameters.Crops.Select(x => x.Trim());
                filter = filter.Where(x => crops.Any(t => x.CropType.Contains(t)));
            }

            if (queryParameters.Brands != null && queryParameters.Brands.Count > 0)
            {
                var brands = queryParameters.Brands.Select(x => x.Trim()).ToArray();
                filter = filter.Where(x => brands.Any(t => x.Brand.Contains(t)));
            }

            if (queryParameters.Solutions != null && queryParameters.Solutions.Count > 0)
            {
                filter = filter.Where(x => queryParameters.Solutions.Any(t => x.Solutions.Any(c => c.Statement.Contains(t))));
            }

            if (queryParameters.Classification != null)
                filter = filter.Where(x => x.Classification == queryParameters.Classification);

            if (queryParameters.DealerIds != null && queryParameters.DealerIds.Length > 0)
            {
                filter = filter.Where(x => queryParameters.DealerIds.Contains(x.Dealer.Id));
            }

            if (queryParameters.DealerName != null)
                filter = filter.Where(x => x.Dealer.FullName.Contains(queryParameters.DealerName));

            if (queryParameters.DealerCompanyName != null)
                filter = filter.Where(x => x.Dealer.Companies.Any(t => t.Name.Contains(queryParameters.DealerCompanyName)));

            if (queryParameters.DealerRegions != null && queryParameters.DealerRegions.Count > 0)
            {
                var regions = queryParameters.DealerRegions.Select(x => x.Trim()).ToArray();
                filter = filter.Where(x => regions.Any(t => x.Dealer.Companies.Any(k => k.Region.Equals(t)) || x.Dealer.Companies.Any(k => k.Branches.Any(f => f.Region.Equals(t)))));
            }

            if (queryParameters.DealerCompanyPhone != null)
                filter = filter.Where(x => x.Dealer.Companies.Any(t => t.Phone.Contains(queryParameters.DealerCompanyPhone)) || x.Dealer.Companies.Any(f => f.Branches.Any(k => k.Phone.Contains(queryParameters.DealerCompanyPhone))));

            var order = queryParameters.OrderBy ?? "Id";
            var orderMode = queryParameters.OrderMode ?? "ASC";
            switch (order)
            {
                case "Id":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.Id) : filter.OrderBy(x => x.Id);
                    break;
                case "Created":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.DateCreated) : filter.OrderBy(x => x.DateCreated);
                    break;
                case "TotalCompanies":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.Dealer.Companies.Count) : filter.OrderBy(x => x.Dealer.Companies.Count);
                    break;
                case "TotalBranches":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.Dealer.Companies.Sum(t => t.Branches.Count)) : filter.OrderBy(x => x.Dealer.Companies.Sum(t => t.Branches.Count));
                    break;
                case "Name":
                    filter = orderMode == "DESC" ? filter.OrderByDescending(x => x.Name) : filter.OrderBy(x => x.Name);
                    break;
            }

            if (queryParameters.CurrentPage != null && queryParameters.ItemsPerPage != null)
            {
                //  Paginated response
                long totalItems = await filter.LongCountAsync();
                var data = await filter.Skip((int)(queryParameters.CurrentPage.Value * queryParameters.ItemsPerPage.Value)).Take((int)queryParameters.ItemsPerPage.Value).ToArrayAsync();
                return Pagination(Map<IEnumerable<ProductModel>>(data), totalItems);
            }
            else
            {
                return Data(Map<IEnumerable<ProductModel>>(await filter.ToArrayAsync()));
            }
        }

        [HttpGet]
        [Route("{id}/user")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<ProductModel>>))]
        public async Task<HttpResponseMessage> GetProductsByUser(long userId, [FromUri]string productType)
        {
            var products = await DB.Products.Where(x => x.Dealer.Id == userId && x.ProductType == productType).ToArrayAsync();
            return Data(Map<IEnumerable<ProductModel>>(products));
        }

        private IList<AppUser> FilterUnique(IEnumerable<AppUser> users)
        {
            List<AppUser> _filtered = new List<AppUser>();
            foreach (var item in users)
            {
                if (!_filtered.Any(x => x.Id == item.Id))
                    _filtered.Add(item);

            }

            return _filtered;
        }

        [HttpGet]
        [Route("summary")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<ProductItemSummary>>))]
        public async Task<HttpResponseMessage> GetProductSummary()
        {
            return Data((await DB.Products.Include(x => x.Dealer)
                .Select(t => new { t.Dealer, t.Id, t.ProductType })
                .GroupBy(x => x.ProductType).ToArrayAsync()).Select(x => new ProductItemSummary()
                {
                    TotalCount = x.LongCount(),
                    ProductType = x.Key,
                    Dealers = Map<IList<ProductDealerInfo>>(FilterUnique(x.Select(t => t.Dealer)))
                }));
        }

        [HttpGet]
        [Route("solutions", Order = 0)]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<string[]>))]
        [NullableArguments()]
        public async Task<HttpResponseMessage> GetSolutions([FromUri]string product = null)
        {
            IQueryable<ProductSolution> solutions = DB.Solutions;
            if (product != null)
                solutions = solutions.Where(x => x.Product.ProductType == product);

            return Data( (await solutions.Select(x => x.Statement).ToArrayAsync()).GroupBy(x => x).Select(x => x.Key).ToArray() );
        }

        [HttpPost]
        [Route("{productId}/lifecylce/{cycleId}/photo")]
        public async Task<HttpResponseMessage> UploadLifeCyclePhoto(long productId, long cycleId)
        {
            var lifeCycle = DB.LifeCycles.FirstOrDefault(x => x.Id == cycleId && x.Product.Id == productId);
            if (lifeCycle == null)
                return NotExists();

            //
            var stream = await Request.Content.ReadAsStreamAsync();
            var contentType = Request.Content.Headers.ContentType.MediaType;

            string tag = MediaHelper.GetUniqueTag();
            var path = MediaHelper.GenerateName(contentType);

            var media = new Media()
            {
                Tag = tag,
                OriginalPath = path.Actual,
                ThumbnailPath = path.Thumbnail
            };

            lifeCycle.Media.Add(media);

            await MediaHelper.SaveMediaAsync(stream, contentType, media);

            await DB.SaveChangesAsync();

            return OperationSuccess("Photo uploaded successfully!");
        }

        [HttpDelete]
        [Route("{productId}/lifecylce/{cycleId}/photo/{tag}")]
        public async Task RemoveLifeCyclePhoto(long productId, long cycleId, string tag)
        {
            var product = await DB.LifeCycles.Include(x => x.Media).FirstOrDefaultAsync(x => x.Id == cycleId && x.Product.Id == productId && x.Media.Any(t => t.Tag == tag));
            if (product != null)
            {
                var media = product.Media.FirstOrDefault(x => x.Tag == tag);

                await MediaHelper.RemoveMediaAsync(media);

                DB.Entry(media).State = EntityState.Deleted;

                await DB.SaveChangesAsync();
            }
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<ProductModel>))]
        public async Task<HttpResponseMessage> Get(long id)
        {
            var product = await DB.Products.FindAsync(id);
            if (product == null)
                return NotExists();

            return Data(Map<ProductModel>(product));
        }

        [HttpPost]
        [Route("{id}/photo")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIResponse))]
        public async Task<HttpResponseMessage> UploadPhoto(long id)
        {
            var product = await DB.Products.FindAsync(id);
            if (product != null)
                return NotExists();

            //
            var stream = await Request.Content.ReadAsStreamAsync();
            var contentType = Request.Content.Headers.ContentType.MediaType;

            string tag = MediaHelper.GetUniqueTag();
            var path = MediaHelper.GenerateName(contentType);

            var media = new Media()
            {
                Tag = tag,
                OriginalPath = path.Actual,
                ThumbnailPath = path.Thumbnail
            };

            product.Media.Add(media);

            await MediaHelper.SaveMediaAsync(stream, contentType, media);

            await DB.SaveChangesAsync();

            return OperationSuccess("Photo uploaded successfully!");
        }

        [Authorize]
        [HttpDelete]
        [Route("{id}/photo/{tag}")]
        public async Task RemovePhotoAsync(long id, string tag)
        {
            var product = await DB.Products.Include(x => x.Media).FirstOrDefaultAsync(x => x.Id == id && x.Media.Any(t => t.Tag == tag));
            if (product != null)
            {
                var media = product.Media.FirstOrDefault(x => x.Tag == tag);

                await MediaHelper.RemoveMediaAsync(media);

                DB.Entry(media).State = EntityState.Deleted;

                await DB.SaveChangesAsync();
            }
        }

        [Authorize]
        [SwaggerResponse(HttpStatusCode.Created, Type = typeof(APIDataResponse<ProductModel>))]
        public async Task<HttpResponseMessage> Post([FromBody]CreateProductModel model)
        {
            var user = await GetCurrentUserAsync();
            if (user.AccountType != GIZ.Models.UserAccountType.Administrator && model.DealerId != user.Id)
            {
                return ValidationError(new ValidationField()
                {
                    Message = "Invalid dealer ID",
                    Property = nameof(model.DealerId),
                    Value = model.DealerId.ToString()
                });
            }

            var dealer = await DB.Users.Where(x => x.AccountType == GIZ.Models.UserAccountType.Dealer).FirstOrDefaultAsync(x => x.Id == model.DealerId);
            if (dealer == null)
                return NotExists("Product does not exists!");

            //  Create product here
            var product = Map<Product>(model);
            dealer.Products.Add(product);

            await DB.SaveChangesAsync();

            return Data(product, HttpStatusCode.Created);
        }

        [Authorize]
        [Route("lifecycle")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIResponse))]
        public async Task<HttpResponseMessage> EditLifeCycle(long id, [FromBody]UpdateLifeCycleModel model)
        {
            var lfc = await DB.LifeCycles
                .Include(x => x.Tags)
               .FirstOrDefaultAsync(x => x.Id == id);

            if (lfc == null)
                return NotExists();

            //
            ObjectUpdater.CopyPropertiesTo(model, lfc, ObjectUpdater.UpdateFlag.DeferUpdateOnNull, only: new[]
            {
                nameof(ProductLifeCycle.Name),
                nameof(ProductLifeCycle.Description),
                nameof(ProductLifeCycle.Order)
            });

            if (model.RemoveTagKeys?.Count > 0)
            {
                await DB.LifeCycleTags.Where(x => model.RemoveTagKeys.Contains(x.Key) && x.LifeCycleId == lfc.Id).DeleteAsync();
            }

            if (model.AddTags?.Count > 0)
            {
                Map<List<LifeCycleTag>>(model.AddTags).ForEach(tag => lfc.Tags.Add(tag));
            }

            //
            await DB.SaveChangesAsync();

            return OperationSuccess("LifeCycle updated succesfully!");
        }

        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIResponse))]
        public async Task<HttpResponseMessage> Put(long id, [FromBody] UpdateProductModel model)
        {
            var product = await DB.Products
                .Include(x => x.Properties)
                .Include(x => x.Solutions)
                .Include(x => x.LifeCycle)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                return NotExists();

            ObjectUpdater.CopyPropertiesTo(model, product, ObjectUpdater.UpdateFlag.DeferUpdateOnNull, new[]
            {
                nameof(Product.Name),
                nameof(Product.Classification),
                nameof(Product.Comments),
            });

            if (model.Brands != null)
                product.Brand = string.Join(",", model.Brands.Select(t => t.Trim()));

            if (model.Crops != null)
                product.CropType = string.Join(",", model.Crops.Select(t => t.Trim()));

            //
            if (model.RemoveLifeCycleIds != null && model.RemoveLifeCycleIds.Count > 0)
            {
                //  remove media for lifecycles
                List<long> clearMedia = new List<long>();
                foreach (var item in DB.LifeCycles.AsNoTracking().Where(x => model.RemoveLifeCycleIds.Contains(x.Id)).Select(x => x.Media))
                {
                    if (item.Count > 0)
                    {
                        await Task.WhenAll(item.Select(x => MediaHelper.RemoveMediaAsync(x)));
                        clearMedia.AddRange(item.Select(t => t.Id));
                    }
                }

                //
                if (clearMedia.Count > 0)
                    await DB.Media.Where(x => clearMedia.Contains(x.Id)).DeleteAsync();

                //
                await DB.LifeCycles.Where(x => model.RemoveLifeCycleIds.Contains(x.Id)).DeleteAsync();
            }

            if (model.RemoveProperties != null && model.RemoveProperties.Count > 0)
                await DB.ProductProperties.Where(x => model.RemoveProperties.Contains(x.Key) && x.ProductId == product.Id).DeleteAsync();

            if (model.RemoveSolutionIds != null && model.RemoveSolutionIds.Count > 0)
                await DB.Solutions.Where(x => model.RemoveSolutionIds.Contains(x.Id)).DeleteAsync();

            if (model.AddLifeCycles?.Count > 0)
                Map<List<ProductLifeCycle>>(model.AddLifeCycles).ForEach(l => product.LifeCycle.Add(l));

            if (model.AddProperties?.Count > 0)
                Map<List<ProductProperty>>(model.AddProperties).ForEach(p => product.Properties.Add(p));

            if (model.AddSolutions?.Count > 0)
                Map<List<ProductSolution>>(model.AddSolutions).ForEach(s => product.Solutions.Add(s));

            await DB.SaveChangesAsync();

            return OperationSuccess("Product updated succesfully!");

        }

        [Authorize]
        public async Task Delete(long id)
        {
            var product = await DB.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product != null)
            {
                product.DateDeleted = DateTime.UtcNow;
                await DB.SaveChangesAsync();
            }

        }

    }
}