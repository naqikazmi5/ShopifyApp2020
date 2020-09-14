using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.ProductHelper
{
    public class ProductDetailModel
    {
        public string shopifyurl { get; set; }
        public string token { get; set; }
        public Product product { get; set; }
        public string productId { get; set; }
        public string apikey { get; set; }
        public string password { get; set; }

    }
}
