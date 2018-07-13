using GIZ.Models.DTO;
using GIZ.Models.Responses;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace GIZ.API.Controllers
{
    [RoutePrefix("api/misc")]
    public class MiscController : BaseController
    {
        protected T LoadModel<T>(string name)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(HostingEnvironment.MapPath($"~/App_Data/{name}")));
        }

        [HttpGet]
        [Route("regions")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<RegionInfo>>))]
        public HttpResponseMessage GetRegions()
        {
            return Data(LoadModel<IList<RegionInfo>>("Regions.json"));
        }

        [HttpGet]
        [Route("products/types")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<ProductTypeInfo>>))]
        public HttpResponseMessage GetProductTypes()
        {
            return Data(LoadModel<IList<ProductTypeInfo>>("ProductTypes.json"));
        }

        [HttpGet]
        [Route("produce")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(APIDataResponse<IEnumerable<ProduceModelInfo>>))]
        public HttpResponseMessage GetProduce()
        {
            return Data(LoadModel<IList<ProduceModelInfo>>("Produce.json"));
        }

    }
}