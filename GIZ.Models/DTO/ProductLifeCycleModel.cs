using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class ProductLifeCycleModel
    {
        public long Id { get; set; }

        public virtual ICollection<LifeCycleTagModel> Tags { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<MediaModel> Media { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime LastUpdated { get; set; }

    }

    public class CreateLifeCycleModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public int Order { get; set; }

        public IList<CreateLifeCycleTagModel> Tags { get; set; }

    }

    public class UpdateLifeCycleModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? Order { get; set; }

        public IList<string> RemoveTagKeys { get; set; }

        public IList<CreateLifeCycleTagModel> AddTags { get; set; }

    }
}
