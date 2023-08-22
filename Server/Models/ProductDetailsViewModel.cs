using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Models
{
    public class ProductDetailsViewModel
    {
        public ProductListViewModel ProductListViewModel { get; set; }
        public List<ProductDetailsFieds> ProductDetailsFieds { get; set; }
        public List<ProductDetailsFiles> ProductFiles { get; set; }
        public bool DisplayContact { get; set; }
        public int StarRateValue { get; set; }
        public pic_user ContactDetails { get; set; }

        public ProductDetailsViewModel()
        {
            ProductDetailsFieds = new List<ProductDetailsFieds>();
            ProductFiles = new List<ProductDetailsFiles>();
        }
    }

    public class ProductDetailsFieds
    {
        public string FieldTitle { get; set; }
        public string FieldValue { get; set; }
    }

    public class ProductDetailsFiles
    {
        public string PicFileUrl { get; set; }
        public string PicFileIconPath { get; set; }
    }

}
