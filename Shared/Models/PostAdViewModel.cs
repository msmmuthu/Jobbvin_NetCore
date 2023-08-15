using System.Collections.Generic;

namespace Jobbvin.Shared.Models
{
    public class PostAdViewModel
    {
        public pic_addpost pic_Addpost { get; set; }
        public List<pic_addpost_field> pic_Addpost_Field { get; set; }
        public List<pic_addpost_locations> pic_Addpost_Locations { get; set; }
  
        public string Pofile_Pic_BaseString { get; set; }
        public string Pofile_Doc_BaseString { get; set; }

        public string Profile_Pic_FileName { get; set; }
        public string Profile_Doc_FileName { get; set; }

    }
}