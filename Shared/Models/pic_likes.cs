﻿using System.ComponentModel.DataAnnotations;

namespace Jobbvin.Shared.Models
{
    public class pic_likes
    {
        public int like_id { get; set; }
        public string likes_product_id { get; set; }
        public string likes_cus_id { get; set; }
        public string? likes_cus_ip { get; set; }
        [Required]
        public string likes_cus_name { get; set; }
        [Required]
        public string likes_cus_mobile { get; set; }
        [Required]
        public string likes_cus_email { get; set; }
        public string likes_ads_user_id { get; set; }
        [Required]
        public string contact_no { get; set; }
    }
}
