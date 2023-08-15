using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Shared.Models
{
    public class pic_calendar
    {
        public int addpost_uni_id { get; set; }
        public int addpost_user_id { get; set; }
        public DateTime Cal_Date { get; set; }
        public int Cal_Available { get; set; }
        public string Cal_Options { get; set; }
    }
}
