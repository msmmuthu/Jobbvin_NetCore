using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Models
{
    public class GetPostAdFieldsViewModel
    {
        public List<pic_categories_fields> pic_categories_fields { get; set; }
        public List<ProductCategoryFields> ProductCategoryFields { get; set; }
        public List<ProductDetailsFiles> ProductFiles { get; set; }
        public List<PostImageFiles> PostImageFiles { get; set; }
        public List<pic_user> ContactDetails { get; set; }
        public int BalanceScheme { get; set; }
        public GetPostAdFieldsViewModel()
        {
            pic_categories_fields = new List<pic_categories_fields>();
            ProductCategoryFields = new List<ProductCategoryFields>();
            ProductFiles = new List<ProductDetailsFiles>();
            PostImageFiles = new List<PostImageFiles>();
            ContactDetails = new List<pic_user>();
        }
    }

    public class ProductCategoryFields
    {
        public int FieldId { get; set; }
        public int FieldDVId { get; set; }
        public int FieldCategoryId { get; set; }
        public string FieldTitle { get; set; }
        public string FieldValue { get; set; }
        public string FieldType { get; set; }
        public bool DisplayInFilter { get; set; }
        public List<ProductCategoryFields> FieldListValues { get; set; }
        public ProductCategoryFields()
        {
            FieldListValues = new List<ProductCategoryFields>();
        }
    }
    public class PostImageFiles
    {
        public string PicFileUrl { get; set; }
    }
}
