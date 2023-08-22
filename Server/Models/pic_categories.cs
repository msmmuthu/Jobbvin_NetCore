using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Models
{
    public class pic_categories
    {
        public int categories_id { get; set; }
        public string categories_name { get; set; }
        public string categories_desc { get; set; }
        public string categories_image { get; set; }
        public int categories_status { get; set; }
        public int categories_parent { get; set; }
        public int categories_sub { get; set; }     
        public string categories_contact_type { get; set; }
        public string cat_search_title { get; set; }
        public string cat_search_limit { get; set; }
        public string user_type { get; set; }
        public int categories_hidden { get; set; }
        public int categories_homepage { get; set; }
        public string categories_price_label { get; set; }
        public int cat_search { get; set; }
        public string cat_fa { get; set; }
        public int categories_maps { get; set; }
        public string categories_desc_label { get; set; }
        public int category_order { get; set; }
    }
}