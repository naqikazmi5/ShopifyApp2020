using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShopifyApp.Models;
using ShopifyApp.Models.AppInstaller;
using ShopifyApp.Models.CustomerHelper;
using ShopifyApp.Models.LocationHelper;
using ShopifyApp.Models.OrderHelper;
using ShopifyApp.Models.ProductHelper;
using ShopifyApp.Models.ShopHelper;
using ShopifySharp;
using ShopifySharp.Enums;
using ShopifySharp.Filters;

namespace ShopifyApp.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        //private readonly IOptions<MyConfig> config;

        //public HomeController(ILogger<HomeController> logger, IOptions<MyConfig> _config)
        //{
        //    _logger = logger;
        //    config = _config;
        //}
        private readonly IOptions<MyConfig> config;
        public HomeController(IOptions<MyConfig> config)
        {
            this.config = config;
        }
        private static string token = "";
        private static string shopifyurl = "";
        private static string apiKey = "";
        private static string apiPassword = "";
        private static bool IsAuth = false;
        private static string shopnamer = "";
        public IActionResult Index()
        {
            if (IsAuth == true)
            {
                ViewBag.Msg = "Application has been succesfully installed and authenticated";
            }
            IsAuth = false;
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult IntegrationStore()
        {
            if (IsAuth == true)
            {
                ViewBag.Msg = "Application has been succesfully installed and authenticated";
            }
            IsAuth = false;
            var requestHeaders = Request.Headers;
            ViewBag.shopname = shopnamer;
            return View();
        }
        public async Task<IActionResult> install(string shop)
        //public async Task<IActionResult> install(int ID , int ChannelID, int PartnerID , int ShipperID,int WarehouseID, string Name, string StoreName, string LocationName,bool InventorySync,bool ProductSync, bool SendSuccessEmail,string SuccessEmail , int IntegrationType,int Duration,string AccessToken,bool SendErrorEmail,string ErrorEmail,bool isEnabled,bool isActive, bool isDeleted, int CreatedBy, int CreatedOn, int UpdatedBy, DateTime UpdatedOn)
        {
            string ApiKey = config.Value.ApiKey;
            string ApiPassword = config.Value.ApiPassword;
            //storeforapps
            //string MyShopifyUrl = "https://ripndip-staging.myshopify.com/";
            //string MyShopifyUrl = "https://netabc.myshopify.com/";
            string MyShopifyUrl = shop;
            string redirectUrl = config.Value.RedirectUrl;
            //redirectUrl = "https://logisticsolutionctkusashopifyapp.azurewebsites.net/Home/auth";
            apiPassword = ApiPassword;
            apiKey = ApiKey;
            shopnamer = shop;
            //An array of the Shopify access scopes your application needs to run.
            var scopes = new List<AuthorizationScope>();
            await Task.Run(() =>
            {
                scopes = new List<AuthorizationScope>()
             {
             AuthorizationScope.ReadProducts,
             AuthorizationScope.WriteProducts,
             AuthorizationScope.ReadCustomers,
             AuthorizationScope.WriteCustomers,
             AuthorizationScope.ReadOrders,
             AuthorizationScope.WriteOrders,
             AuthorizationScope.ReadInventory,
             AuthorizationScope.WriteInventory
             };
            });
            string state = "";
            var authUrl = AuthorizationService.BuildAuthorizationUrl(scopes, MyShopifyUrl, ApiKey, redirectUrl);
            return Redirect(authUrl.AbsoluteUri);
            //return View();
        }
        public async Task<IActionResult> auth(string shop, string code, string hmac, string state)
       {
            shopnamer = shop;
            string shopname = "";
            var requestHeaders = Request.Headers;
            var redirecttype = requestHeaders.FirstOrDefault(x => x.Key == "Sec-Fetch-Dest").Value.FirstOrDefault();
            //var redirecttype1 = requestHeaders.FirstOrDefault(x => x.Key == "Sec-Fetch-Dest");
            //redirecttype1.Value = "iframe";
            if (!string.IsNullOrEmpty(shop) && !string.IsNullOrEmpty(code))
            {
                InstallResponse response = new InstallResponse();
                string[] shosp = shop.Split(".");
                shopname = shosp[0];
                string accessToken = await AuthorizationService.Authorize(code, shop, apiKey, apiPassword);
                token = accessToken;
                shopifyurl = shop;
                //await CreateUninstallHook(shopifyurl, token);
            }
            if (redirecttype == "iframe")
            {
                //return RedirectToAction("IntegrationStore");
                return RedirectToAction("Index");
            }
            await StoreLocations(shop, token);
            await InstallResponse(shopname, token);
            //response.StoreLocations = await StoreLocations();
            //return View();
            IsAuth = true;
            var AbsoluteUri = $"https://{shop}/admin/apps/{apiKey}";
            //return StatusCode(200);
            return Redirect(AbsoluteUri);
        }
        [HttpGet]
        public async Task<IActionResult> SyncData(string StoreName,string AccessToken, int Duration , bool InventorySync, bool ProductSync, bool CustomerSync, bool OrderSync)
        {
            StoreName = "netabc";
            string shopifyurl = $"https://{StoreName}.myshopify.com/";
            AccessToken = "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";
            InstallResponse response = new InstallResponse();
            await CreateUninstallHook(shopifyurl, AccessToken);
            response.CustomerSync = await SyncCustomers(shopifyurl, AccessToken);
            response.ProductSync = await SyncProducts(shopifyurl, AccessToken);
            response.OrderSync = await SyncOrders(shopifyurl, AccessToken, Duration);
            //if (CustomerSync == true)
            //{
            //    response.CustomerSync = await SyncCustomers(shopifyurl, AccessToken);
            //}
            //if (ProductSync == true)
            //{
            //    response.ProductSync = await SyncProducts(shopifyurl, AccessToken);
            //}
            //if (OrderSync == true)
            //{
            //    response.OrderSync = await SyncOrders(shopifyurl, AccessToken, Duration);
            //}
            response.WebhookCreated = await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/uninstall", "app/uninstalled");
            //await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/addcustomer", "customers/create");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/addcustomer", "customers/create");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/addcustomer", "customers/update");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/deletecustomer", "customers/delete");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/addproduct", "products/create");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/addproduct", "products/update");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/deleteproduct", "products/delete");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/addorder", "orders/create");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/addorder", "orders/updated");
            await CreateWebHook(shopifyurl, AccessToken, $"{config.Value.RootUrl}/home/deleteorder", "orders/delete");
            response.StoreLocations = await StoreLocations(shopifyurl, AccessToken);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> uninstall()
        {
            var requestHeaders = Request.Headers;
            var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
            string[] shop = shoper.Split(".");
            string shopname = shop[0];
            await UnInstallResponse(shopname);
            var req = HttpContext.Request.Body;
            var json = new StreamReader(req).ReadToEnd();
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> testing()
        {
            var requestHeaders = Request.Headers;
            var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
            string[] shop = shoper.Split(".");
            string shopname = shop[0];
            string requestBody = null;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> addcustomer()
        {
            try
            {
                var requestHeaders = Request.Headers;
                var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
                string[] shop = shoper.Split(".");
                string shopname = shop[0];
                string requestBody = null;
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                string shopifySecretKey = config.Value.ApiPassword;
                if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
                {

                    var client = new HttpClient();
                    List<CustomerReturnModel> customers = new List<CustomerReturnModel>();
                    var json = requestBody;
                    CustomerReturnModel customer = JsonConvert.DeserializeObject<CustomerReturnModel>(json);
                    customer.StoreName = shopname;
                    customers.Add(customer);
                    string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyCustomers";
                    // strong typed instance
                    ProductDetailModel pdm = new ProductDetailModel();
                    pdm.shopifyurl = shopifyurl;
                    pdm.token = token;
                    var response2 = client.PostAsJsonAsync(apiUrl2, customers).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                }
                else
                {
                    //Webhook is not authentic and should not be acted on.
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                return StatusCode(500);
            }
           
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> updatecustomer()
        {
            var requestHeaders = Request.Headers;
            string requestBody = null;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            string shopifySecretKey = config.Value.ApiPassword;
            if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
            {
                //Webhook is authentic.
                var json = requestBody;
            }
            else
            {
                //Webhook is not authentic and should not be acted on.
            }
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> DelCustData()
        {
            var client = new HttpClient();
            var json = "";
            CustomerRedactedWebhook customer = JsonConvert.DeserializeObject<CustomerRedactedWebhook>(json);
            string apiUrl2 = $"{config.Value.ResponseUrl}/api/CustomerRedact";
            // strong typed instance
            ProductDetailModel pdm = new ProductDetailModel();
            pdm.shopifyurl = shopifyurl;
            pdm.token = token;
            var response2 = client.PostAsJsonAsync(apiUrl2, customer).Result;
            response2.EnsureSuccessStatusCode();
            string responseBody1 = await response2.Content.ReadAsStringAsync();
            //var requestHeaders = Request.Headers;
            //var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
            //string[] shop = shoper.Split(".");
            //string shopname = shop[0];
            //string requestBody = null;
            //using (StreamReader reader = new StreamReader(Request.Body))
            //{
            //    requestBody = await reader.ReadToEndAsync();
            //}
            //string shopifySecretKey = "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";
            //if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
            //{
            //    var client = new HttpClient();
            //    var json = requestBody;
            //    CustomerRedactedWebhook customer = JsonConvert.DeserializeObject<CustomerRedactedWebhook>(json);
            //    string apiUrl2 = $"{config.Value.ResponseUrl}/api/CustomerRedact";
            //    // strong typed instance
            //    ProductDetailModel pdm = new ProductDetailModel();
            //    pdm.shopifyurl = shopifyurl;
            //    pdm.token = token;
            //    var response2 = client.PostAsJsonAsync(apiUrl2, customer).Result;
            //    response2.EnsureSuccessStatusCode();
            //    string responseBody1 = await response2.Content.ReadAsStringAsync();
            //}
            //else
            //{
            //    //Webhook is not authentic and should not be acted on.
            //}
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> CustData()
        {
            var client = new HttpClient();
            var json = "";
            CustomerDataRequestWebhook customer = JsonConvert.DeserializeObject<CustomerDataRequestWebhook>(json);
            string apiUrl2 = $"{config.Value.ResponseUrl}/api/GetCustomerData";
            // strong typed instance
            ProductDetailModel pdm = new ProductDetailModel();
            pdm.shopifyurl = shopifyurl;
            pdm.token = token;
            var response2 = client.PostAsJsonAsync(apiUrl2, customer).Result;
            response2.EnsureSuccessStatusCode();
            string responseBody1 = await response2.Content.ReadAsStringAsync();
            //var requestHeaders = Request.Headers;
            //var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
            //string[] shop = shoper.Split(".");
            //string shopname = shop[0];
            //string requestBody = null;
            //using (StreamReader reader = new StreamReader(Request.Body))
            //{
            //    requestBody = await reader.ReadToEndAsync();
            //}
            //string shopifySecretKey = "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";
            //if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
            //{
            //    var client = new HttpClient();
            //    var json = requestBody;
            //    CustomerDataRequestWebhook customer = JsonConvert.DeserializeObject<CustomerDataRequestWebhook>(json);
            //    string apiUrl2 = $"{config.Value.ResponseUrl}/api/GetCustomerData";
            //    // strong typed instance
            //    ProductDetailModel pdm = new ProductDetailModel();
            //    pdm.shopifyurl = shopifyurl;
            //    pdm.token = token;
            //    var response2 = client.PostAsJsonAsync(apiUrl2, customer).Result;
            //    response2.EnsureSuccessStatusCode();
            //    string responseBody1 = await response2.Content.ReadAsStringAsync();
            //}
            //else
            //{
            //    //Webhook is not authentic and should not be acted on.
            //}
            return StatusCode(200);
        }
        [HttpGet]
        public async Task<IActionResult> DelShopData()
        {
            var client = new HttpClient();
            var json = "";
            //ShopRedactedWebhook shopdata = JsonConvert.DeserializeObject<ShopRedactedWebhook>(json);
            ShopRedactedWebhook shopdata = new ShopRedactedWebhook();
            shopdata.ShopDomain = "netabc.myshopify.com";
            string apiUrl2 = $"{config.Value.ResponseUrl}/api/ShopRedact";
            // strong typed instance
            var response2 = client.PostAsJsonAsync(apiUrl2, shopdata).Result;
            response2.EnsureSuccessStatusCode();
            string responseBody1 = await response2.Content.ReadAsStringAsync();
            //var requestHeaders = Request.Headers;
            //var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
            //string[] shop = shoper.Split(".");
            //string shopname = shop[0];
            //string requestBody = null;
            //using (StreamReader reader = new StreamReader(Request.Body))
            //{
            //    requestBody = await reader.ReadToEndAsync();
            //}
            //string shopifySecretKey = "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";
            //if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
            //{
            //    var client = new HttpClient();
            //    var json = requestBody;
            //    ShopRedactedWebhook customer = JsonConvert.DeserializeObject<ShopRedactedWebhook>(json);
            //    string apiUrl2 = $"{config.Value.ResponseUrl}/api/ShopRedactedWebhook";
            //    // strong typed instance
            //    ProductDetailModel pdm = new ProductDetailModel();
            //    pdm.shopifyurl = shopifyurl;
            //    pdm.token = token;
            //    var response2 = client.PostAsJsonAsync(apiUrl2, customer).Result;
            //    response2.EnsureSuccessStatusCode();
            //    string responseBody1 = await response2.Content.ReadAsStringAsync();
            //}
            //else
            //{
            //    //Webhook is not authentic and should not be acted on.
            //}
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> deletecustomer()
        {
            try
            {
                var requestHeaders = Request.Headers;
                var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
                string[] shop = shoper.Split(".");
                string shopname = shop[0];
                string requestBody = null;
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                string shopifySecretKey = config.Value.ApiPassword;
                if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
                {

                    var client = new HttpClient();
                    List<CustomerReturnModel> customers = new List<CustomerReturnModel>();
                    var json = requestBody;
                    CustomerReturnModel customer = JsonConvert.DeserializeObject<CustomerReturnModel>(json);
                    customer.StoreName = shopname;
                    customers.Add(customer);
                    string apiUrl2 = $"{config.Value.ResponseUrl}/api/DeleteShopifyCutomers";
                    var response2 = client.PostAsJsonAsync(apiUrl2, customers).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                }
                else
                {
                    //Webhook is not authentic and should not be acted on.
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                return StatusCode(500);
            }

            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> addproduct()
        {
            try
            {
                var requestHeaders = Request.Headers;
                var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
                string[] shop = shoper.Split(".");
                string shopname = shop[0];
                string requestBody = null;
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                string shopifySecretKey = config.Value.ApiPassword;
                if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
                {
                    ShopInfo shopInfo = new ShopInfo();
                    shopInfo.ShopName = shopname;
                    var client1 = new HttpClient();
                    string apiUrl = $"{config.Value.ResponseUrl}/api/ShopifyToken";
                    var response = client1.PostAsJsonAsync(apiUrl, shopInfo).Result;
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var readString = JObject.Parse(responseBody)["data"];
                    string token = readString.ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        List<ProductReturnModel> products = new List<ProductReturnModel>();
                        List<Product> prds = new List<Product>();
                        var json = requestBody;
                        ProductReturnModel product = JsonConvert.DeserializeObject<ProductReturnModel>(json);
                        ProductDetailModel pdm = new ProductDetailModel();
                        pdm.productId = product.Id.ToString();
                        pdm.shopifyurl = shoper;
                        pdm.token = token;
                        var prd = await new ProductHandler().GetProduct(pdm);
                        prds.Add(prd);
                        var client = new HttpClient();
                        products = await new ProductHandler().GetProductReturnModel(prds, shopname);
                        string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyProducts";
                        string output = JsonConvert.SerializeObject(products);
                        var response2 = client.PostAsJsonAsync(apiUrl2, products).Result;
                        response2.EnsureSuccessStatusCode();
                        string responseBody1 = await response2.Content.ReadAsStringAsync();
                    }
                       
                }
                else
                {
                    //Webhook is not authentic and should not be acted on.
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> updateproduct()
        {
            var requestHeaders = Request.Headers;
            string requestBody = null;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            string shopifySecretKey = config.Value.ApiPassword;
            if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
            {
                //Webhook is authentic.
                var json = requestBody;
            }
            else
            {
                //Webhook is not authentic and should not be acted on.
            }
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> deleteproduct()
        {
            try
            {
                var requestHeaders = Request.Headers;
                var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
                string[] shop = shoper.Split(".");
                string shopname = shop[0];
                string requestBody = null;
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                string shopifySecretKey = config.Value.ApiPassword;
                if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
                {
                    List<long> ids = new List<long>();
                    var client = new HttpClient();
                    List<ProductReturnModel> products = new List<ProductReturnModel>();
                    var json = requestBody;
                    ProductReturnModel product = JsonConvert.DeserializeObject<ProductReturnModel>(json);
                    product.StoreName = shopname;
                    products.Add(product);
                    long id = (long)Convert.ToDouble(product.Id);
                    ids.Add(id);
                    string apiUrl2 = $"{config.Value.ResponseUrl}/api/DeleteShopifyProducts";
                    var response2 = client.PostAsJsonAsync(apiUrl2, ids).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                }
                else
                {
                    //Webhook is not authentic and should not be acted on.
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> addorder()
        {
            try
            {
                var requestHeaders = Request.Headers;
                var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
                string[] shop = shoper.Split(".");
                string shopname = shop[0];
                string requestBody = null;
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                string shopifySecretKey = config.Value.ApiPassword;
                if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
                {
                    var client = new HttpClient();
                    List<OrderReturnModel> orders = new List<OrderReturnModel>();
                    var json = requestBody;
                    OrderReturnModel order = JsonConvert.DeserializeObject<OrderReturnModel>(json);
                    order.StoreName = shopname;
                    orders.Add(order);
                    string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyOrders";
                    var response2 = client.PostAsJsonAsync(apiUrl2, orders).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                }
                else
                {
                    //Webhook is not authentic and should not be acted on.
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> updateorder()
        {
            var requestHeaders = Request.Headers;
            string requestBody = null;
            using (StreamReader reader = new StreamReader(Request.Body))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            string shopifySecretKey = config.Value.ApiPassword;
            if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
            {
                //Webhook is authentic.
                var json = requestBody;
            }
            else
            {
                //Webhook is not authentic and should not be acted on.
            }
            return StatusCode(200);
        }
        [HttpPost]
        public async Task<IActionResult> deleteorder()
        {
            try
            {
                var requestHeaders = Request.Headers;
                var shoper = requestHeaders.FirstOrDefault(x => x.Key == "X-Shopify-Shop-Domain").Value.FirstOrDefault();
                string[] shop = shoper.Split(".");
                string shopname = shop[0];
                string requestBody = null;
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                string shopifySecretKey = config.Value.ApiPassword;
                if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
                {
                    List<long> ids = new List<long>();
                    var client = new HttpClient();
                    List<OrderReturnModel> orders = new List<OrderReturnModel>();
                    var json = requestBody;
                    OrderReturnModel order = JsonConvert.DeserializeObject<OrderReturnModel>(json);
                    order.StoreName = shopname;
                    orders.Add(order);
                    long id = (long)Convert.ToDouble(order.Id);
                    ids.Add(id);
                    string apiUrl2 = $"{config.Value.ResponseUrl}/api/DeleteShopifyOrders";
                    var response2 = client.PostAsJsonAsync(apiUrl2, ids).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                }
                else
                {
                    //Webhook is not authentic and should not be acted on.
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }

        public string ReturnToken()
        {
            return token;
        }
        public async Task<string> Allhooks()
        {
            var service = new WebhookService(shopifyurl, token);
            var webhooks = await service.ListAsync();
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
        public async Task<bool> SyncCustomers(string shopifyurl, string token)
        {
            bool response;
            try
            {
                string[] shosp = shopifyurl.Split(".");
                string shopname = shosp[0];
                CustomerDetailModel cdm = new CustomerDetailModel();
                cdm.token = token;
                cdm.shopifyurl = shopifyurl;
                List<Customer> customersList = await new CustomerHandler().GetAllCustomers(cdm);
                List<CustomerReturnModel> customerReturns = await new CustomerHandler().GetCustomerReturnModel(customersList, shopname);
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(customerReturns);
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyCustomers";
                var response2 = client.PostAsJsonAsync(apiUrl2, customerReturns).Result;
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response2.Content.ReadAsStringAsync();
                var readString = JObject.Parse(responseBody1)["status"];
                response = Convert.ToBoolean(readString);
            }
            catch (Exception)
            {
                return false;
            }
            return response;
        }
        public async Task<bool> SyncProducts(string shopifyurl, string token)
        {
            bool response;
            try
            {
                shopifyurl = "netabc.myshopify.com";
                token = "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";
                string[] shosp = shopifyurl.Split(".");
                string shopname = shosp[0];
                var client = new HttpClient();
                ProductDetailModel model = new ProductDetailModel();
                model.shopifyurl = shopifyurl;
                model.token = token;
                List<Product> products = await new ProductHandler().GetAllProducts(model);
                //string msgs = await new ProductHandler().AddMetaFeilds(products, shopifyurl, token);
                List<ProductReturnModel> productReturns = await new ProductHandler().GetProductReturnModel(products, shopname);
                //List<ProductReturnModel> newlist = new List<ProductReturnModel>();
                //newlist.Add(productReturns.FirstOrDefault());
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyProducts";
                string json = JsonConvert.SerializeObject(productReturns);
                var response2 = client.PostAsJsonAsync(apiUrl2, productReturns).Result;
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response2.Content.ReadAsStringAsync();
                var readString = JObject.Parse(responseBody1)["status"];
                response = Convert.ToBoolean(readString);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                return false;
            }
            return response;
        }
        public async Task<bool> SyncOrders(string shopifyurl, string token,int duration)
        {
            bool response;
            try
            {
                string[] shosp = shopifyurl.Split(".");
                string shopname = shosp[0];
                var client = new HttpClient();
                OrderDetailModel model = new OrderDetailModel();
                model.shopifyurl = shopifyurl;
                model.token = token;
                model.duration = duration;
                List<Order> orders = await new OrderHandler().GetAllOrders(model);
                string json = JsonConvert.SerializeObject(orders);
                List<OrderReturnModel> orderReturns = await new OrderHandler().GetOrderReturnModel(orders, shopname);
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyOrders";
                var response2 = client.PostAsJsonAsync(apiUrl2, orderReturns).Result;
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response2.Content.ReadAsStringAsync();
                var readString = JObject.Parse(responseBody1)["status"];
                response = Convert.ToBoolean(readString);
            }
            catch (Exception)
            {
                return false;
            }

            return response;
        }
        public async Task<bool> InstallResponse(string shopname,string token)
        {
            bool response;
            try
            {
                ShopInfo shopInfo = new ShopInfo();
                shopInfo.ShopName = shopname;
                shopInfo.Token = token;
                var client = new HttpClient();
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/ShopifyInstallation";
                var response2 = client.PostAsJsonAsync(apiUrl2, shopInfo).Result;
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
        public async Task<bool> UnInstallResponse(string shopname)
        {
            bool response;
            try
            {
                ShopInfo shopInfo = new ShopInfo();
                shopInfo.ShopName = shopname;
                var client = new HttpClient();
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/ShopifyUnInstallation";
                var response2 = client.PostAsJsonAsync(apiUrl2, shopInfo).Result;
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
        public async Task<bool> StoreLocations(string shopifyurl, string token)
        {
            bool response;
            try
            {
                string[] shosp = shopifyurl.Split(".");
                string shopname = shosp[0];
                var service = new LocationService(shopifyurl, token);
                var locations = await service.ListAsync();
                var client = new HttpClient();
                List<LocationReturnModel> locationReturns = await new LocationHandler().GetLocationReturnModel(locations, shopname);
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
        public async Task<IActionResult> ShopifyIntegration(IntegrationHelper data)
        {
            var msg = "";
            try
            {
                // string shopname = data.storename;
                string shopname = data.storename;
                int duration = data.duration;
                bool product = data.productsync;
                bool order = data.ordersync;
                bool customer = data.customersync;
                ShopInfo shopInfo = new ShopInfo();
                shopInfo.ShopName = shopname;
                var client = new HttpClient();
                string apiUrl2 = $"{config.Value.ResponseUrl}/api/ShopifyToken";
                var response2 = client.PostAsJsonAsync(apiUrl2, shopInfo).Result;
                response2.EnsureSuccessStatusCode();
                string responseBody1 = await response2.Content.ReadAsStringAsync();
                var readString = JObject.Parse(responseBody1)["data"];
                string token = readString.ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    if (product == true)
                    {
                        bool response = await new ProductHandler().SyncProduct(shopname, token, config.Value.ResponseUrl);
                    }
                    if (customer == true)
                    {
                        bool response = await new CustomerHandler().SyncCustomer(shopname, token, config.Value.ResponseUrl);
                    }
                    if (order == true)
                    {
                        bool response = await new OrderHandler().SyncOrders(shopname, token, config.Value.ResponseUrl, duration);
                    }
                    msg = "Request has been started, data will be synced within few minutes.";
                }
            }
            catch (Exception ex)
            {
                msg = "error";
                throw;
            }
            return Json(msg);
        }
        public async Task<IActionResult> testproduct()
        {
            try
            {
                var requestHeaders = Request.Headers;
                var shoper = "netabc.myshopify.com";
                string[] shop = shoper.Split(".");
                string shopname = shop[0];
                string requestBody = null;
                //using (StreamReader reader = new StreamReader(Request.Body))
                //{
                //    requestBody = await reader.ReadToEndAsync();
                //}
                //string shopifySecretKey = "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";
                //if (AuthorizationService.IsAuthenticWebhook(requestHeaders, requestBody, shopifySecretKey))
                //{
                    var client = new HttpClient();
                    List<ProductReturnModel> products = new List<ProductReturnModel>();
                    var json = requestBody;
                    ProductReturnModel product = JsonConvert.DeserializeObject<ProductReturnModel>(json);
                    product.StoreName = shopname;
                    products.Add(product);
                    string apiUrl2 = $"{config.Value.ResponseUrl}/api/AddShopifyProducts";
                    string output = JsonConvert.SerializeObject(products);
                    var response2 = client.PostAsJsonAsync(apiUrl2, products).Result;
                    response2.EnsureSuccessStatusCode();
                    string responseBody1 = await response2.Content.ReadAsStringAsync();
                //}
                //else
                //{
                //    //Webhook is not authentic and should not be acted on.
                //}
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return StatusCode(200);
        }
    }
}
