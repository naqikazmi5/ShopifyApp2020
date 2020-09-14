using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.ProductHelper
{
    public class VariantInventory
    {
        public string StoreName { get; set; }
        public string Token { get; set; }
        public int? InventoryQuantity { get; set; }
        public Nullable<long> InventoryItemId { get; set; }
        public Nullable<long> StorageLocationID { get; set; }
    }
}
