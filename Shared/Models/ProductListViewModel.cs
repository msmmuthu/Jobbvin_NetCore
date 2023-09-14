using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Shared.Models
{
    public class ProductListViewModel
    {
        public int pic_ads_id { get; set; }
        public int pic_user_id { get; set; }
        public string pic_title { get; set; }
        public string pic_discription { get; set; }
        public string pic_add_taluk { get; set; }
        public string pic_post_city { get; set; }
        public string pic_file_url { get; set; }
        public string pic_user_fullname { get; set; }
        public string addpost_status { get; set; }
        public string pic_category { get; set; }
        public int pic_categoryId { get; set; }
        public DateTime pic_postdate { get; set; }
        public double pic_price { get; set; }
        public List<string> pic_add_images { get; set; }
        public StarRating pic_star_rating { get; set; }
        public int EnableCalender { get; set; }
        public int DisplayOnHomePage { get; set; }
        public string pic_locations { get; set; }
        public bool DisplayContact { get; set; }
        public List<pic_addpost_locations> pic_Addpost_Locations { get; set; }
        public List<DisplayInFieldValue> DisplayInFieldValues { get; set; }

        public ProductListViewModel()
        {
            pic_add_images = new List<string>();
            pic_Addpost_Locations = new List<pic_addpost_locations>();
            DisplayInFieldValues = new List<DisplayInFieldValue>();
        }
    }

    public class StarRating
    {
        public double Average { get; set; }
        public int TotalRatings { get; set; }

    }
}
