using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIZ.Models.DTO
{
    public class ProduceModelInfo
    {
        public string Name { get; set; }

        public IList<string> Regions { get; set; }
    }
}
