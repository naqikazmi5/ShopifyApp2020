using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.LocationHelper
{
    public class LocationReturnModel
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphQLAPIId { get; set; }
        [JsonProperty("active")]
        public bool? Active { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("address1")]
        public string Address1 { get; set; }
        [JsonProperty("address2")]
        public string Address2 { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("province")]
        public string Province { get; set; }
        [JsonProperty("province_code")]
        public string ProvinceCode { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("country_name")]
        public string CountryName { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        [JsonProperty("legacy")]
        public bool? Legacy { get; set; }
        [JsonProperty("store_name")]
        public string StoreName { get; set; }
    }
}
