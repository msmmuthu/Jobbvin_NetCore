﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Shared.Models
{
    public class pic_user
    {
        public int user_id { get; set; }
        public string user_username { get; set; }
        public string user_password { get; set; }
        public string user_email { get; set; }
        public string user_mobile { get; set; }
        public string user_city { get; set; }
        public string user_refer { get; set; }
        public string user_type { get; set; }
        public int user_status { get; set; }
        public string mobile_val { get; set; }
        public int mobile_status { get; set; }
        public string user_id_unique { get; set; }
        public string user_sex { get; set; }
        public byte[] user_pic { get; set; }
        public string user_pic_url { get; set; }
        public string user_pic_BaseString { get; set; }
        public DateTime user_dob { get; set; }
    }
}