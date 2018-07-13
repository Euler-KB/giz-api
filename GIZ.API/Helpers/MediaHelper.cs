using GIZ.API.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GIZ.API.Helpers
{
    public static class MediaHelper
    {
        public class NameInfo
        {
            public string Actual { get; set; }

            public string Thumbnail { get; set; }
        }

        public static readonly int DefaultThumbnailWidth = 200;

        public static readonly int DefaulThumbnailtHeight = 200;

        public static readonly string DefaultTag = "";

        public static string GetUniqueTag()
        {
            return string.Concat((Guid.NewGuid().ToString("N").OrderBy(x => Guid.NewGuid()))).Substring(0, 12);
        }

        private static Task SaveThumbnailAsync(Stream imageStream, int width, int height, string path)
        {
            return Task.Run(() =>
            {
                Image image = Image.FromStream(imageStream);
                using (var bitmap = new Bitmap(width, height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(image, 0, 0, width, height);
                    }

                    using (var fs = new FileStream(path, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write))
                        bitmap.Save(fs, ImageFormat.Png);
                }

            });

        }

        /// <summary>
        /// Generate new media name
        /// </summary>
        public static NameInfo GenerateName(string mimeType)
        {
            var _prefix = Guid.NewGuid().ToString("N");
            string extension = mimeType.Split('/').LastOrDefault();
            return new NameInfo()
            {
                Actual = $"{_prefix}.{extension}",
                Thumbnail = $"{_prefix}_thumb.{extension}"
            };
        }


        /// <summary>
        /// Saves the media
        /// </summary>
        /// <param name="stream">The stream of the media</param>
        /// <param name="mimeType">The content type of the media</param>
        public static Task SaveMediaAsync(Stream stream, string mimeType, Media media)
        {
            return SaveMediaAsync(stream, mimeType, media.OriginalPath, media.ThumbnailPath);
        }

        /// <summary>
        /// Saves the media
        /// </summary>
        /// <param name="stream">The stream of the media</param>
        /// <param name="mimeType">The content type of the media</param>
        /// <param name="thumbs">Save thumbnail image as well</param>
        public static async Task SaveMediaAsync(Stream stream, string mimeType, string originalName, string thumbnailName = null)
        {
            using (var fs = new FileStream(originalName, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }

            if (thumbnailName != null)
            {
                stream.Seek(0, SeekOrigin.Begin);

                //  save thumbnail
                await SaveThumbnailAsync(stream, DefaultThumbnailWidth, DefaulThumbnailtHeight, thumbnailName);
            }
        }

        public static Task RemoveMediaAsync(Media media)
        {
            var operations = new List<Task>
            {
                RemoveMediaAsync(media.OriginalPath)
            };

            if (media.ThumbnailPath != null)
                operations.Add(RemoveMediaAsync(media.ThumbnailPath));

            return Task.WhenAll(operations.ToArray());
        }

        public static Task RemoveMediaAsync(string pathName)
        {
            return Task.Run(() =>
            {
                string absolutePath = Handlers.StaticMediaHandler.ResolvePath(pathName);

                if (File.Exists(absolutePath))
                    File.Delete(absolutePath);
            });

        }

    }
}