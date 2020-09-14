using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.CustomerHelper
{
    public class CustomerDetailModel
    {
        public string shopifyurl { get; set; }
        public string token { get; set; }
        public Customer customer { get; set; }
        public string customerId { get; set; }
        public string apikey { get; set; }
        public string password { get; set; }
    }
}
