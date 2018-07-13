using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class MediaModel
    {
        public string OriginalPath { get; set; }

        public string ThumbnailPath { get; set; }

        public string Tag { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
