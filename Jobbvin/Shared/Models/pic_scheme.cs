using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Shared.Models
{
    public class pic_scheme
    {
        public int scheme_id { get; set; }
        public string scheme_name { get; set; }
        public string scheme_desc { get; set; }
        public int scheme_ads_qty { get; set; }
        public int scheme_price { get; set; }
        public DateTime scheme_date { get; set; }
        public int scheme_photo { get; set; }
        public int scheme_valid { get; set; }
        public int scheme_status { get; set; }

    }
}
