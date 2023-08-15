using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Shared.Models
{
    public class temp_user
    {
        public int user_id { get; set; }
        public string user_username { get; set; }
        public string user_password { get; set; }
        public string user_email { get; set; }
        public string user_mobile { get; set; }
        public string user_city { get; set; }
        public string user_type { get; set; }
        public int user_status { get; set; }
        public string mobile_val { get; set; }
        public int mobile_status { get; set; }
        public string user_id_unique { get; set; }
    }
}
