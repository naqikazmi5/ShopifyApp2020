using Newtonsoft.Json;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.ProductHelper
{
    public class ProductReturnModel
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphQLAPIId { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("body_html")]
        public string BodyHtml { get; set; }
        [JsonProperty("created_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonProperty("updated_at", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? UpdatedAt { get; set; }
        [JsonProperty("published_at", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        public DateTimeOffset? PublishedAt { get; set; }
        [JsonProperty("vendor")]
        public string Vendor { get; set; }
        [JsonProperty("product_type")]
        public string ProductType { get; set; }
        [JsonProperty("handle")]
        public string Handle { get; set; }
        [JsonProperty("template_suffix")]
        public string TemplateSuffix { get; set; }
        [JsonProperty("published_scope")]
        public string PublishedScope { get; set; }
        [JsonProperty("tags")]
        public string Tags { get; set; }
        [JsonProperty("variants")]
        public IEnumerable<ProductVariant> Variants { get; set; }
        [JsonProperty("options")]
        public IEnumerable<ProductOption> Options { get; set; }
        [JsonProperty("images")]
        public IEnumerable<ProductImage> Images { get; set; }
        [JsonProperty("metafields")]
        public IEnumerable<MetaField> Metafields { get; set; }
        /// <summary>
        /// The name of the store of the product.
        /// </summary>
        [JsonProperty("store_name")]
        public string StoreName { get; set; }
        public string shopifyurl { get; set; }
        public string token { get; set; }
    }
    public class ProductList
    {
        public List<Product> products { get; set; }
    }
}
