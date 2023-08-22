using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Models
{
    public class pic_categories_fields
    {
        public int displayinlist { get; set; }
        public int fields_categories_id { get; set; }
        public int fields_id { get; set; }
        public string fields_title { get; set; }
        public string fields_type { get; set; }
        public string field_chain_value { get; set; }
        public int field_DV_id { get; set; }     
        public int field_priority { get; set; }
        public int field_quickedit { get; set; }
        public string field_sample { get; set; }
        public string field_value { get; set; }
        public int multi { get; set; }
    }
}