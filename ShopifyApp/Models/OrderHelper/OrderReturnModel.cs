using Newtonsoft.Json;
using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.OrderHelper
{
    public class OrderReturnModel
    {
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphQLAPIId { get; set; }
        [JsonProperty("number")]
        public int? Number { get; set; }
        [JsonProperty("order_number")]
        public int? OrderNumber { get; set; }
        [JsonProperty("order_status_url")]
        public string OrderStatusUrl { get; set; }
        [JsonProperty("payment_gateway_names")]
        public IEnumerable<string> PaymentGatewayNames { get; set; }
        [JsonProperty("processed_at")]
        public DateTimeOffset? ProcessedAt { get; set; }
        [JsonProperty("processing_method")]
        public string ProcessingMethod { get; set; }
        [JsonProperty("referring_site")]
        public string ReferringSite { get; set; }
        [JsonProperty("refunds")]
        public IEnumerable<Refund> Refunds { get; set; }
        [JsonProperty("shipping_address")]
        public Address ShippingAddress { get; set; }
        [JsonProperty("shipping_lines")]
        public IEnumerable<ShippingLine> ShippingLines { get; set; }
        [JsonProperty("source_name")]
        public string SourceName { get; set; }
        [JsonProperty("note_attributes")]
        public IEnumerable<NoteAttribute> NoteAttributes { get; set; }
        [JsonProperty("subtotal_price")]
        public decimal? SubtotalPrice { get; set; }
        [JsonProperty("taxes_included")]
        public bool? TaxesIncluded { get; set; }
        [JsonProperty("test")]
        public bool? Test { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("total_discounts")]
        public decimal? TotalDiscounts { get; set; }
        [JsonProperty("total_line_items_price")]
        public decimal? TotalLineItemsPrice { get; set; }
        [JsonProperty("total_tip_received")]
        public decimal? TotalTipReceived { get; set; }
        [JsonProperty("total_price")]
        public decimal? TotalPrice { get; set; }
        [JsonProperty("total_tax")]
        public decimal? TotalTax { get; set; }
        [JsonProperty("total_weight")]
        public long? TotalWeight { get; set; }
        [JsonProperty("updated_at")]
        public DateTimeOffset? UpdatedAt { get; set; }
        [JsonProperty("user_id")]
        public long? UserId { get; set; }
        [JsonProperty("tax_lines")]
        public IEnumerable<TaxLine> TaxLines { get; set; }
        [JsonProperty("transactions")]
        public IEnumerable<Transaction> Transactions { get; set; }
        [JsonProperty("note")]
        public string Note { get; set; }
        [JsonProperty("location_id")]
        public long? LocationId { get; set; }
        [JsonProperty("app_id")]
        public long? AppId { get; set; }
        [JsonProperty("billing_address")]
        public Address BillingAddress { get; set; }
        [JsonProperty("browser_ip")]
        public string BrowserIp { get; set; }
        [JsonProperty("buyer_accepts_marketing")]
        public bool? BuyerAcceptsMarketing { get; set; }
        [JsonProperty("cancel_reason")]
        public string CancelReason { get; set; }
        [JsonProperty("cancelled_at")]
        public DateTimeOffset? CancelledAt { get; set; }
        [JsonProperty("cart_token")]
        public string CartToken { get; set; }
        [JsonProperty("client_details")]
        public ClientDetails ClientDetails { get; set; }
        [JsonProperty("closed_at")]
        public DateTimeOffset? ClosedAt { get; set; }
        [JsonProperty("created_at")]
        public DateTimeOffset? CreatedAt { get; set; }
        [JsonProperty("currency")]
        public string Currency { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("customer")]
        public Customer Customer { get; set; }
        [JsonProperty("device_id")]
        public long? DeviceId { get; set; }
        [JsonProperty("discount_codes")]
        public IEnumerable<DiscountCode> DiscountCodes { get; set; }
        [JsonProperty("discount_applications")]
        public IEnumerable<DiscountApplication> DiscountApplications { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("financial_status")]
        public string FinancialStatus { get; set; }
        [JsonProperty("fulfillments")]
        public IEnumerable<Fulfillment> Fulfillments { get; set; }
        [JsonProperty("fulfillment_status")]
        public string FulfillmentStatus { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("tags")]
        public string Tags { get; set; }
        [JsonProperty("landing_site")]
        public string LandingSite { get; set; }
        [JsonProperty("line_items")]
        public IEnumerable<LineItem> LineItems { get; set; }
        [JsonProperty("customer_locale")]
        public string CustomerLocale { get; set; }
        [JsonProperty("metafields")]
        public IEnumerable<MetaField> Metafields { get; set; }
        /// <summary>
        /// The name of the store for the order.
        /// </summary>
        [JsonProperty("store_name")]
        public string StoreName { get; set; }
    }
    public class OrderList
    {
        public List<Order> orders { get; set; }
    }
}
