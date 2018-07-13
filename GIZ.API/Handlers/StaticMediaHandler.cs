using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace GIZ.API.Handlers
{
    public class StaticMediaHandler : HttpTaskAsyncHandler
    {
        static readonly string RootPath;

        static StaticMediaHandler()
        {
            RootPath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["MEDIA_DIR"]);

            //  Always ensure directory is set
            Directory.CreateDirectory(RootPath);
        }

        public static string ResolvePath(string name)
        {
            return Path.Combine(RootPath, name);
        }

        private Models.GIZContext dbContext = new Models.GIZContext();

        public override bool IsReusable => true;

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            var request = context.Request;
            var response = context.Response;
            var mediaName = request.Url.Segments.Last();
            var media = await dbContext.Media.AsNoTracking().FirstOrDefaultAsync(x => x.OriginalPath == mediaName || x.ThumbnailPath == mediaName);
            if (media != null)
            {
                string filePath = ResolvePath(mediaName);
                if (File.Exists(filePath))
                {
                    await Task.Run(() => response.WriteFile(filePath));
                    return;
                }

            }

            //
            response.StatusDescription = "No such media exists!";
            response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}