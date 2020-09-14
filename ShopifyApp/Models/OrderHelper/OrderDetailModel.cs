using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.OrderHelper
{
    public class OrderDetailModel
    {
        public string shopifyurl { get; set; }
        public string token { get; set; }
        public Order order { get; set; }
        public string orderId { get; set; }
        public int  duration { get; set; }
        public string apikey { get; set; }
        public string password { get; set; }
    }
}
