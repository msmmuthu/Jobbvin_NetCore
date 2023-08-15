using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Shared.Models
{
    public class PoductFilterModel
    {
        public int cat_id { get; set; }
        public int type { get; set; }
        public int sort { get; set; }
        public int offset { get; set; }

    }
}
