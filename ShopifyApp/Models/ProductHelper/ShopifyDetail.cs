using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.ProductHelper
{
    public class ShopifyDetail
    {
        public int ID { get; set; }
        public int? ChannelID { get; set; }
        public int PartnerID { get; set; }
        public int? ShipperID { get; set; }
        public int WarehouseID { get; set; }
        public string Name { get; set; }
        public string StoreName { get; set; }
        public string LocationName { get; set; }
        public bool InventorySync { get; set; }
        public bool ProductSync { get; set; }
        public bool SendSuccessEmail { get; set; }
        public string SuccessEmail { get; set; }
        public int IntegrationType { get; set; }
        public int Duration { get; set; }
        public string AccessToken { get; set; }
        public bool SendErrorEmail { get; set; }
        public string ErrorEmail { get; set; }
        public bool? isEnabled { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool CustomerSync { get; set; }
        public bool OrderSync { get; set; }
        public string ApiKey { get; set; }
        public string Password { get; set; }
    }
}
