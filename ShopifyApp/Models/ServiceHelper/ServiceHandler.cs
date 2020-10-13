using AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopifyApp.Models.OrderHelper;
using ShopifyApp.Models.ProductHelper;
using ShopifySharp;
using ShopifySharp.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShopifyApp.Models.ServiceHelper
{
    public class ServiceHandler
    {
    
        public async Task<string> SyncStoreOrders()
        {
            string msg = "";
            var storesinfo = await GetStoresInfo();
            return msg;
        }
        public async Task<string> SyncStoreData()
        {
            string msg = "Data Synced Successfuly!";
            try
            {
                var client1 = new HttpClient();
                string apiUrl = $"{WebUtil.ResponseUrl}/api/GetShopifyStoreInfo";
                var response = client1.GetAsync(apiUrl).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                StoreResponseModel storeinfo = JsonConvert.DeserializeObject<StoreResponseModel>(responseBody);
                foreach (var item in storeinfo?.data)
                {
                    if (!string.IsNullOrEmpty(item.Duration) && !string.IsNullOrEmpty(item.StoreName) && !string.IsNullOrEmpty(item.Token))
                    {
                          string shopifyurl = $"https://{item.StoreName}.myshopify.com/";
                        if (item.ProductSunc == "true")
                        {
                           await Task.Run(() => SyncProductsData(shopifyurl, item.StoreName, item.Token));
                        }
                        if (item.Inventory == "true")
                        {
                            await Task.Run(() => SyncInventory(item.StoreName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return msg;
        }
        public async Task<string> GetStoresInfo()
        {
            string msg = "Data Synced Successfuly!";
            try
            {
                var client1 = new HttpClient();
                string apiUrl = $"{WebUtil.ResponseUrl}/api/GetShopifyStoreInfo";
                var response = client1.GetAsync(apiUrl).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                StoreResponseModel storeinfo = JsonConvert.DeserializeObject<StoreResponseModel>(responseBody);
                foreach (var item in storeinfo?.data)
                {
                    if (!string.IsNullOrEmpty(item.Duration) && !string.IsNullOrEmpty(item.StoreName) && !string.IsNullOrEmpty(item.Token))
                    {
                        if (item.StoreName == "zafarproducts")
                        {
                           await Task.Run(() => SyncOrderData(item.StoreName, item.Token, item.Duration));
                        }
                       
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
             return msg;
        }
        public async Task<string> SyncOrderData(string storename, string token , string duration)
        {
            string msg = "error";
            try
            {
                var client = new HttpClient();
                bool response;
                string shopifyurl = $"https://{storename}.myshopify.com/";
                OrderDetailModel odm = new OrderDetailModel();
                odm.shopifyurl = shopifyurl;
                odm.token = token;
                odm.duration = Convert.ToInt32(duration);
                var orders = await new OrderHandler().GetAllOrders(odm);
                List<OrderReturnModel> orderReturns = await new OrderHandler().GetOrderReturnModel(orders, storename);
                string json = JsonConvert.SerializeObject(orders);
                foreach (var item in orderReturns)
                {
                    List<OrderReturnModel> orm = new List<OrderReturnModel>();
                    orm.Add(item);
                    string jjj = JsonConvert.SerializeObject(orm);
                    string apiUrl2 = $"{WebUtil.ResponseUrl}/api/AddShopifyOrders";
                    var response2 = client.PostAsJsonAsync(apiUrl2, orm).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                    var readString = JObject.Parse(responseBody1)["status"];
                    response = Convert.ToBoolean(readString.ToString());
                    if (response == true)
                    {
                        msg = "success";
                    }
                }
            }
            catch (Exception ex)
            {

                msg = ex.Message.ToString();
            }
            return msg;
        }
        public async Task<bool> SyncProductsData(string shopifyurl, string storename, string token)
        {
            bool response = true;
            try
            {
                var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(30);
                ProductDetailModel model = new ProductDetailModel();
                model.shopifyurl = shopifyurl;
                model.token = token;
                List<Product> products = await new ProductHandler().GetAllProducts(model);
                List<ProductReturnModel> productsList = await new ProductHandler().GetProductReturnModel(products, storename);
                string apiUrl2 = $"{WebUtil.ResponseUrl}/api/AddShopifyProducts";
                string json = JsonConvert.SerializeObject(productsList);
                foreach (var item in productsList)
                {
                    List<ProductReturnModel> prdlist = new List<ProductReturnModel>();
                    prdlist.Add(item);
                    var response2 = client.PostAsJsonAsync(apiUrl2, prdlist).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                    var readString = JObject.Parse(responseBody1)["status"];
                    response = Convert.ToBoolean(readString.ToString());
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                return false;
            }
            return response;
        }
        public async Task<bool> SyncInventory(string storename)
        {
            try
            {
                var client1 = new HttpClient();
                string apiUrl = $"{WebUtil.ResponseUrl}/api/PostProductsInventory?storeName={storename} ";
                var response = client1.GetAsync(apiUrl).Result;
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return true;

            }
            catch (Exception ex)
            {
            }
            return true;
        }
        }
}
