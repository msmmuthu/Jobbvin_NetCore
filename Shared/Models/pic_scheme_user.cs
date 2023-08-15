using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Shared.Models
{
    public class pic_scheme_user
    {
        public int pic_scheme_user_id { get; set; }
        public int pic_scheme_id { get; set; }
        public string pic_scheme_name { get; set; }
        public string pic_scheme_desc { get; set; }
        public int total_ads { get; set; }
        public int cost_scheme { get; set; }
        public string payment_status { get; set; }
        public string payment_method { get; set; }
        public int pic_scheme_balance_qty { get; set; }
        public int pic_user_id { get; set; }

        public DateTime scheme_purchased_date { get; set; }
        public DateTime scheme_approval_date { get; set; }
        public string scheme_purpose { get; set; }
        public string scheme_cash_id { get; set; }
        public string payment_details { get; set; }
        public string payment_remarks { get; set; }
        public int photo_limit { get; set; }
        public int ads_valid { get; set; }
        public DateTime scheme_expiry { get; set; }
    }
    public class Pic_Ads_Count
    {
        public int TotalSchemes { get; set; }
        public int TotalAds { get; set; }
        public int TotalUsedAds { get; set; }
        public int TotalBalanceAds { get; set; }

    }

    public class SchemeListViewModel
    {
        public bool DisplaySubmitButton { get; set; }
        public List<SchemeListModel> schemeListModels { get; set; }
        public SchemeListViewModel()
        {
            schemeListModels = new List<SchemeListModel>();
        }
    }



    public class SchemeListModel
    {
        public int SchemeId { get; set; }

        public string SchemeName { get; set; }
        public string SchemeDescription { get; set; }
        public int Price { get; set; }
        public int Select { get; set; }
        public string Purpose { get; set; } //Reques/Post
        public string PaymentDetails { get; set; }

    }
}
