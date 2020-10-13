using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.ShopHelper
{
    public class IntegrationHelper
    {
        public string storename { get; set; }
        public string location { get; set; }
        public int duration { get; set; }
        public bool ordersync { get; set; }
        public bool productsync { get; set; }
        public bool customersync { get; set; }


    }
}
