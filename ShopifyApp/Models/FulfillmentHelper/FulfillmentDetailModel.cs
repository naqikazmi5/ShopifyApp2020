using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.FulfillmentHelper
{
    public class FulfillmentDetailModel
    {
        public string token { get; set; }
        public string shopifyurl { get; set; }
        public string trackingCompany { get; set; }
        public string trackingUrl { get; set; }
        public string trackingNumber { get; set; }
        public long orderId { get; set; }
        public long fulfillmentId { get; set; }
        public List<LineItem> lineItem { get; set; }
    }
}
