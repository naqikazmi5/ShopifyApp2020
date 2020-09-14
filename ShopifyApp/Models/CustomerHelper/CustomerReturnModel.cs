using Newtonsoft.Json;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.CustomerHelper
{
    public class CustomerReturnModel
    {
        //public Customer();
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphQLAPIId { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        [JsonProperty("total_spent")]
        public decimal? TotalSpent { get; set; }
        [JsonProperty("tax_exempt")]
        public bool? TaxExempt { get; set; }
        [JsonProperty("tags")]
        public string Tags { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("orders_count")]
        public int? OrdersCount { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        [JsonProperty("last_order_name")]
        public string LastOrderName { get; set; }
        [JsonProperty("last_order_id")]
        public long? LastOrderId { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("multipass_identifier")]
        public string MultipassIdentifier { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("default_address")]
        public Address DefaultAddress { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonProperty("addresses")]
        public IEnumerable<Address> Addresses { get; set; }
        [JsonProperty("accepts_marketing")]
        public bool? AcceptsMarketing { get; set; }
        [JsonProperty("verified_email")]
        public bool? VerifiedEmail { get; set; }
        [JsonProperty("metafields")]
        public IEnumerable<MetaField> Metafields { get; set; }
        /// <summary>
        /// The name of the store of the product.
        /// </summary>
        [JsonProperty("store_name")]
        public string StoreName { get; set; }
    }
    public class CustomerList
    {
        public List<Customer> customers { get; set; }
    }
}
