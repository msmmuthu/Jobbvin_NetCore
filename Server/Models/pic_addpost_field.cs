using System;

namespace Jobbvin.Server.Models
{
    public class pic_addpost_files
    {
        public int id { get; set; }
        public int pic_ads_id { get; set; }
        public int pic_file_url { get; set; }
        public DateTime pic_file_added_on { get; set; }
    }
}
