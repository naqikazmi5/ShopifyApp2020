using AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopifySharp;
using ShopifySharp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShopifyApp.Models.OrderHelper
{
    public class OrderHandler
    {
        public async Task<List<Order>> GetAllOrders(OrderDetailModel model)
        {
            string msg;
            List<Order> Orders = new List<Order>();
            try
            {
                var allOrders = new List<Order>();
                var service = new OrderService(model.shopifyurl, model.token);
                //if (model.duration == 0)
                //{
                //    model.duration = 3;
                //}
                var page = await service.ListAsync(new OrderListFilter
                {
                    Limit = 250,
                    FulfillmentStatus = "Unfulfilled",
                    FinancialStatus = "paid",
                    CreatedAtMin = DateTime.Today.AddDays(-model.duration),
                    CreatedAtMax = DateTime.Now

                });

                while (true)
                {
                    allOrders.AddRange(page.Items);

                    if (!page.HasNextPage)
                    {
                        break;
                    }

                    page = await service.ListAsync(page.GetNextPageFilter());
                }
                Orders = allOrders;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return Orders;
        }
        public async Task<List<OrderReturnModel>> GetOrderReturnModel(List<Order> orders, string shopname)
        {
            List<OrderReturnModel> orderReturns = new List<OrderReturnModel>();
            foreach (var item in orders)
            {
                OrderReturnModel returnModel = new OrderReturnModel
                {
                    Id = item.Id,
                    CreatedAt = item.CreatedAt,
                    AppId = item.AppId,
                    BillingAddress = item.BillingAddress,
                    BuyerAcceptsMarketing = item.BuyerAcceptsMarketing,
                    CancelledAt = item.CancelledAt,
                    ClosedAt = item.ClosedAt,
                    DiscountApplications = item.DiscountApplications,
                    NoteAttributes = item.NoteAttributes,
                    ProcessedAt = item.ProcessedAt,
                    ShippingAddress = item.ShippingAddress,
                    UpdatedAt = item.UpdatedAt,
                    BrowserIp = item.BrowserIp,
                    CancelReason = item.CancelReason,
                    CartToken = item.CartToken,
                    ClientDetails = item.ClientDetails,
                    Currency = item.Currency,
                    Customer = item.Customer,
                    CustomerLocale = item.CustomerLocale,
                    DeviceId = item.DeviceId,
                    DiscountCodes = item.DiscountCodes,
                    Email = item.Email,
                    FinancialStatus = item.FinancialStatus,
                    Fulfillments = item.Fulfillments,
                    FulfillmentStatus = item.FulfillmentStatus,
                    LandingSite = item.LandingSite,
                    LineItems = item.LineItems,
                    LocationId = item.LocationId,
                    Metafields = item.Metafields,
                    Name = item.Name,
                    Note = item.Note,
                    Number = item.Number,
                    OrderNumber = item.OrderNumber,
                    OrderStatusUrl = item.OrderStatusUrl,
                    PaymentGatewayNames = item.PaymentGatewayNames,
                    Phone = item.Phone,
                    ProcessingMethod = item.ProcessingMethod,
                    ReferringSite = item.ReferringSite,
                    Refunds = item.Refunds,
                    ShippingLines = item.ShippingLines,
                    SourceName = item.SourceName,
                    StoreName = shopname,
                    SubtotalPrice = item.SubtotalPrice,
                    Tags = item.Tags,
                    TaxesIncluded = item.TaxesIncluded,
                    TaxLines = item.TaxLines,
                    Test = item.Test,
                    Token = item.Token,
                    TotalDiscounts = item.TotalDiscounts,
                    TotalLineItemsPrice = item.TotalLineItemsPrice,
                    TotalPrice = item.TotalPrice,
                    TotalTax = item.TotalTax,
                    TotalTipReceived = item.TotalTipReceived,
                    TotalWeight = item.TotalWeight,
                    Transactions = item.Transactions,
                    UserId = item.UserId
                };
                orderReturns.Add(returnModel);
            }
            return orderReturns;
        }
        public async Task<bool> SyncOrders(string shopname, string token, string serverurl,int duration)
        {
            bool response = true;
            try
            {
                string shopifyurl = $"https://{shopname}.myshopify.com/";
                var client = new HttpClient();
                OrderDetailModel model = new OrderDetailModel();
                model.shopifyurl = shopifyurl;
                model.token = token;
                model.duration = duration;
                List<Order> orders = await GetAllOrders(model);
                string json = JsonConvert.SerializeObject(orders);
                List<OrderReturnModel> orderReturns = await GetOrderReturnModel(orders, shopname);
                string apiUrl2 = $"{serverurl}/api/AddShopifyOrders";
                var response2 = client.PostAsJsonAsync(apiUrl2, orderReturns).Result;
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response2.Content.ReadAsStringAsync();
                var readString = JObject.Parse(responseBody1)["status"];
                response = Convert.ToBoolean(readString.ToString());
            }
            catch (Exception)
            {

                throw;
            }
            return response;
        }
    }
}
