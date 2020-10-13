using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.CustomerHelper
{
    public class ContactDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Email { get; set; }
        public string MonthlyOrder { get; set; }
        public string OrderType { get; set; }
        public string Message { get; set; }
    }
}
