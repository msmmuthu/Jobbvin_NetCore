using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Models
{
    public class pic_addpost
    {
        public int pic_id { get; set; }
        public int pic_ads_id { get; set; }
        public string pic_title { get; set; }
        public int pic_category { get; set; }
        public int pic_price { get; set; }
        public string pic_discription { get; set; }
        public DateTime pic_postdate { get; set; }
        public int pic_is_freeads { get; set; }
        public int addpost_scheme_user_id { get; set; }
        public string pic_user_email { get; set; }
        public int pic_user_id { get; set; }
        public string pic_post_city { get; set; }
        public string pic_user_mobile { get; set; }
        public string pic_user_fullname { get; set; }
        public int pic_user_type { get; set; }
        public string pic_user_address { get; set; }
        public string pic_tag { get; set; }
        public string pic_add_taluk { get; set; }
        public string pic_add_town { get; set; }
        public double pic_add_lan { get; set; }
        public double pic_add_lon { get; set; }
        public string pic_admin_tag { get; set; }

        public int addpost_status { get; set; }
        public int pic_request { get; set; }
        public int pic_sms { get; set; }
        public int pic_privacy { get; set; }

        public string pic_refer_id { get; set; }
        public string pic_special { get; set; }
        public string pic_map_lan { get; set; }
        public string pic_map_lon { get; set; }

        public DateTime pic_validity { get; set; }
        public int pic_validity_auto { get; set; }
        public int pic_qty { get; set; }
        public string pic_multi_loc_chip { get; set; }
        public bool EnableCalender { get; set; }
        public bool DisplayOnHomePage { get; set; }
    }
}
