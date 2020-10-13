using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.ServiceHelper
{
    public class StoreResponseModel
    {
        public List<StoreDetailModel> data { get; set; }
        public string status { get; set; }
        public string message { get; set; }
    }
    public class StoreDetailModel
    {
        public string StoreName { get; set; }
        public string Token  { get; set; }
        public string Inventory  { get; set; }
        public string ProductSunc  { get; set; }
        public string Duration  { get; set; }

    }
}
