using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopifyApp.Models.AppInstaller;
using ShopifyApp.Models.CustomerHelper;
using ShopifyApp.Models.LocationHelper;
using ShopifyApp.Models.OrderHelper;
using ShopifyApp.Models.ProductHelper;
using ShopifySharp;
using ShopifySharp.Enums;
using ShopifySharp.Filters;

namespace ShopifyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IOptions<MyConfig> config;
        public ProductController(IOptions<MyConfig> config)
        {
            this.config = config;
        }
        //create product in shopify
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody]ProductDetailModel model)
        {
            var response = "";
            try
            {
                //model.token = new HomeController().ReturnToken();
                var service = new ProductService(model.shopifyurl, model.token);
                for (int i = 0; i < 1000; i++)
                {
                    var product = new Product()
                    {
                        Title = "Product with metafield",
                        Vendor = "Burton",
                        BodyHtml = "<strong>Good snowboard!</strong>",
                        ProductType = "Snowboard",
                        Images = new List<ProductImage>
    {
        new ProductImage
        {
            Attachment = "R0lGODlhAQABAIAAAAAAAAAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw=="
        }
    },
                    };
                    product = await service.CreateAsync(product);
                var mservice = new MetaFieldService(model.shopifyurl, model.token);
                var metafield = new MetaField()
                {
                    Namespace = "myNamespace",
                    Key = "myKey",
                    Value = "5",
                    ValueType = "integer",
                    Description = "This is a test meta field. It is an integer value."
                };

                //Create a new metafield on a product
                //metafield = await mservice.CreateAsync(metafield,Convert.ToInt64(product.Id), "products");
                //var value = await mservice.GetAsync(Convert.ToInt64(metafield.Id));
                //product = await service.GetAsync(Convert.ToInt64(product.Id));
                }
                //var task = Task.Run(async () => await mservice.ListAsync(Convert.ToInt64(product.Id), "products"));
                //var result = task.Result;
                //By default, creating a product will publish it. To create an unpublished product:+1:
                //product = await service.CreateAsync(product, new ProductCreateOptions() { Published = false });
                response = "Product is added successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Update Product
        [Route("[action]")]
        public async Task<IActionResult> UpdateProduct([FromBody]ProductDetailModel model)
        {
            var response = "";
            try
            {
                long pid =Convert.ToInt64(model.productId);
                //model.token = new HomeController().ReturnToken();
                var service = new ProductService(model.shopifyurl, model.token);
                var product = await service.UpdateAsync(pid, new Product()
                {
                    Title = "Burton Custom Freestlye 151",
                    Vendor = "Burton",
                    BodyHtml = "<strong>Good snowboard!</strong>",
                    ProductType = "Snowboard",
                    Images = new List<ProductImage>
    {
        new ProductImage
        {
            Attachment = "R0lGODlhAQABAIAAAAAAAAAAACH5BAEAAAAALAAAAAABAAEAAAICRAEAOw=="
        }
    },
                });
                response = "Product is updated successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(response);
        }
        //Get Product By Id
        [Route("[action]")]
        public async Task<IActionResult> GetProduct([FromBody]ProductDetailModel model)
        {
            dynamic response;
            try
            {
                //model.token = new HomeController().ReturnToken();
                long pid = Convert.ToInt64(model.productId);
                var service = new ProductService(model.shopifyurl, model.token);
                response = await service.GetAsync(pid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Delete Product By Id
        [Route("[action]")]
        public async Task<IActionResult> DeleteProduct([FromBody]ProductDetailModel model)
        {
            var response = "";
            try
            {
                //model.token = new HomeController().ReturnToken();
                long pid = Convert.ToInt64(model.productId);
                var service = new ProductService(model.shopifyurl, model.token);
                await service.DeleteAsync(pid);
                response = "Product is deleted successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Get All Products
        [Route("[action]")]
        public async Task<IActionResult> GetAllProducts(string token, string shopifyurl)
        {
            dynamic response = null;
            try
            {
                //token = new HomeController().ReturnToken();
                var allProducts = new List<Product>();
                var service = new ProductService(shopifyurl,token);
                var page = await service.ListAsync(new ProductListFilter
                {
                    Limit = 250,
                });

                while (true)
                {
                    allProducts.AddRange(page.Items);

                    if (!page.HasNextPage)
                    {
                        break;
                    }

                    page = await service.ListAsync(page.GetNextPageFilter());
                }
                response = allProducts;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }

        //SendProductData
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendProductData([FromBody]Product model)
        {
            string response = "";
            try
            {
                response = "Successfuly done!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //update product inventory in shopify
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> AddShopifyInventory([FromBody]List<VariantInventory> products)
        {
            var response = "";
            try
            {
                long locationid =0;
                VariantInventory model = products.FirstOrDefault();
                string shopifyurl = $"https://{model.StoreName}.myshopify.com/";
                var locationservice = new LocationService(shopifyurl, model.Token);
                var locations = await locationservice.ListAsync();
                var lid = locations.Where(x => x.Name.ToLower() == "parco").FirstOrDefault();
                if (lid != null)
                {
                    locationid = Convert.ToInt64(lid.Id);
                }
                //model.Token = "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";
                var inventoryservice = new InventoryLevelService(shopifyurl, model.Token);
                //locationid for parco
                //long locationid = Convert.ToInt64("45679804460"); 
                //var id1 = Convert.ToInt64("36306970148908");
                //var id2 = Convert.ToInt64("36306970181676");
                //var id3 = Convert.ToInt64("36306970214444");
                //var id4 = Convert.ToInt64("36306982731820");
                var ids = new List<long>();
                //ids.Add(id1);
                //ids.Add(id2);
                //ids.Add(id3);
                //ids.Add(id4);
                foreach (var item in products)
                {
                    ids.Add( Convert.ToInt64(item.InventoryItemId));
                }
                var inventoryLevelFilter = new InventoryLevelListFilter()
                {
                    InventoryItemIds = ids
                };
                var levels = await inventoryservice.ListAsync(inventoryLevelFilter);
                foreach (var item in levels.Items)
                {
                    if (item.LocationId == locationid)
                    {
                        var iteminfo = products.Where(x => x.InventoryItemId == item.InventoryItemId).FirstOrDefault();
                        item.Available = iteminfo.InventoryQuantity;
                        var service = await inventoryservice.SetAsync(item);
                    }
                  
                }
                //var level = levels.Items.FirstOrDefault();
                //if (level != null)
                //{
                //    level.Available = 26;
                //    var service = await inventoryservice.SetAsync(level);
                //}
                //ShopifySharp.ProductVariant pv = new ShopifySharp.ProductVariant();
                //pv.InventoryQuantity = 500;
                //foreach (var item in products)
                //{
                //    foreach (var variant in item.Variants)
                //    {
                //        var variantid = Convert.ToInt64(variant.InventoryItemId);/*Convert.ToInt64("36257378205740");*/  
                //        var ids = new List<long>()
                //        {
                //           variantid
                //        };
                //        var inventoryLevelFilter = new InventoryLevelListFilter()
                //        {
                //            InventoryItemIds = ids
                //        };
                //        var levels = await inventoryservice.ListAsync(inventoryLevelFilter);
                //        var level = levels.Items.FirstOrDefault();
                //        if (level!=null)
                //        {
                //            level.Available = 26;
                //            var service = await inventoryservice.SetAsync(level);
                //        }

                //    }



                //    //var service = new ProductService(model.shopifyurl, model.token);
                //    //List<Product> retlist = await new ProductHandler().ReturnProductModel(products);
                //    //foreach (var item in retlist)
                //    //{
                //    //    var a = item.Variants.FirstOrDefault();
                //    //    a.InventoryItemId = Convert.ToInt64("36257378205740");
                //    //    a.InventoryQuantity = 500;
                //    //    a.InventoryManagement = "shopify";
                //    //    a.InventoryPolicy = "deny";
                //    //    var msg = await service.UpdateAsync(Convert.ToInt64(item.Id), item);
                //    //}
                    response = "success";
                //}
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //update product inventory in shopify
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> SyncData([FromBody] ShopifyDetail model)
        {
            SyncResponse response = new SyncResponse();
            try
            {
                string shopifyurl = $"https://{model.StoreName}.myshopify.com";
                if (model.CustomerSync == true)
                {
                    response.CustomerSync = await SyncCustomers(shopifyurl, model.StoreName, model.AccessToken);
                }
                if (model.ProductSync == true)
                {
                    response.ProductSync = await SyncProducts(shopifyurl, model.StoreName, model.AccessToken);
                }
                if (model.Duration != 0 )
                {
                    response.OrderSync = await SyncOrders(shopifyurl, model.StoreName, model.AccessToken, model.Duration);
                }
                await CreateUninstallHook(shopifyurl, model.AccessToken);
                //response.CustomerSync = await SyncCustomers();
                //response.ProductSync = await SyncProducts();
                //response.OrderSync = await SyncOrders();
                //response.WebhookCreated = await CreateWebHook(shopifyurl, token, $"{config.Value.RootUrl}/home/uninstall", "app/uninstalled");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/testing", "product_metafields/update");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addcustomer", "customers/create");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addcustomer", "customers/update");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/deletecustomer", "customers/delete");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addproduct", "products/create");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addproduct", "products/update");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/deleteproduct", "products/delete");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addorder", "orders/create");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addorder", "orders/updated");
                await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/deleteorder", "orders/delete");
                //response.WebhookCreated = await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/uninstall", "app/uninstalled");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addcustomer", "customers/create");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addcustomer", "customers/update");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/deletecustomer", "customers/delete");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addproduct", "products/create");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addproduct", "products/update");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/deleteproduct", "products/delete");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addorder", "orders/create");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/addorder", "orders/updated");
                //await CreateWebHook(shopifyurl, model.AccessToken, $"{config.Value.RootUrl}/home/deleteorder", "orders/delete");
                //response.StoreLocations = await StoreLocations(shopifyurl, model.StoreName, model.AccessToken);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        public async Task<string> CreateUninstallHook(string shopifyurl, string token)
        {
            var service = new WebhookService(shopifyurl, token);
            var webhooks = await service.ListAsync();
            if (webhooks.Items?.FirstOrDefault() != null)
            {
                foreach (var item in webhooks.Items)
                {
                    long itemid = Convert.ToInt64(item.Id);
                    await service.DeleteAsync(itemid);
                }
            }
            return "";
        }
        public async Task<bool> CreateWebHook(string shopifyurl, string token, string Address, string Topic)
        {
            try
            {
                var service = new WebhookService(shopifyurl, token);
                Webhook hook = new Webhook()
                {
                    Address = Address, // Address
                    CreatedAt = DateTime.Now,
                    //Fields = new List<string>() { "id", "updated_at","FirstName" },
                    Format = "json",
                    //MetafieldNamespaces = new List<string>() { "metafield1", "metafield2" },
                    Topic = Topic, // Topic
                };
                hook = await service.CreateAsync(hook);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> SyncCustomers(string shopifyurl, string storename, string token)
        {
            bool response = true;
            try
            {
                var client = new HttpClient();
                CustomerDetailModel cdm = new CustomerDetailModel();
                cdm.token = token;
                cdm.shopifyurl = shopifyurl;
                List<Customer> customersList = await new CustomerHandler().GetAllCustomers(cdm);
                List<CustomerReturnModel> customerReturns = await new CustomerHandler().GetCustomerReturnModel(customersList, storename);
                string json = JsonConvert.SerializeObject(customerReturns);
                foreach (var item in customerReturns)
                {
                List<CustomerReturnModel> crm = new List<CustomerReturnModel>();
                crm.Add(item);
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyCustomers";
                var response2 = client.PostAsJsonAsync(apiUrl2, crm).Result;
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response2.Content.ReadAsStringAsync();
                var readString = JObject.Parse(responseBody1)["status"];
                response = Convert.ToBoolean(readString.ToString());
                }
            }
            catch (Exception)
            {
                return false;
            }
            return response;
        }
        public async Task<bool> SyncProducts(string shopifyurl, string storename, string token)
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
                //string msgs = await new ProductHandler().AddMetaFeilds(products, shopifyurl, token);
                List<ProductReturnModel> productsList = await new ProductHandler().GetProductReturnModel(products, storename);
                //List<ProductReturnModel> newlist = new List<ProductReturnModel>();
                //newlist.Add(productReturns.FirstOrDefault());
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyProducts";
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
                //var response2 = client.PostAsJsonAsync(apiUrl2, productsList).Result;
                
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                return false;
            }
            return response;
        }
        public async Task<bool> SyncOrders(string shopifyurl, string storename, string token, int duration)
        {
            bool response = true;
            try
            {
                var client = new HttpClient();
                OrderDetailModel model = new OrderDetailModel();
                model.shopifyurl = shopifyurl;
                model.token = token;
                model.duration = duration;
                List<Order> orders = await new OrderHandler().GetAllOrders(model);
                List<OrderReturnModel> orderReturns = await new OrderHandler().GetOrderReturnModel(orders, storename);
                string json = JsonConvert.SerializeObject(orders);
                foreach (var item in orderReturns)
                {
                    List<OrderReturnModel> orm = new List<OrderReturnModel>();
                    orm.Add(item);
                    string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyOrders";
                    var response2 = client.PostAsJsonAsync(apiUrl2, orm).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                    var readString = JObject.Parse(responseBody1)["status"];
                    response = Convert.ToBoolean(readString.ToString());
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return response;
        }
        public async Task<bool> StoreLocations(string shopifyurl, string storename, string token)
        {
            bool response;
            try
            {
                var service = new LocationService(shopifyurl, token);
                var locations = await service.ListAsync();
                var client = new HttpClient();
                List<LocationReturnModel> locationReturns = await new LocationHandler().GetLocationReturnModel(locations, storename);
                string json = JsonConvert.SerializeObject(locationReturns);
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyLocations";
                var response2 = client.PostAsJsonAsync(apiUrl2, locationReturns).Result;
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response2.Content.ReadAsStringAsync();
                var readString = JObject.Parse(responseBody1)["status"];
                response = Convert.ToBoolean(readString.ToString());
            }
            catch (Exception)
            {
                return false;
            }
            return response;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> TestProductData([FromBody] ProductReturnModel model)
        {
            string response = "";
            try
            {
                var client = new HttpClient();
                List<ProductReturnModel> products = new List<ProductReturnModel>();
                //var json = requestBody;
                //ProductReturnModel product = JsonConvert.DeserializeObject<ProductReturnModel>(json);
                model.StoreName = "netabc";
                products.Add(model);
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyProducts";
                string output = JsonConvert.SerializeObject(products);
                var response2 = client.PostAsJsonAsync(apiUrl2, products).Result;
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response2.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
    }
}