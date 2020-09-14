using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ShopifyApp.Models.ProductHelper;
using ShopifySharp;

namespace ShopifyApp.Controllers
{
    public class TestController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var token = "";
            var shopifyurl = "";
            //values.Add("shopifyurl", shopifyurl);
            var client = new HttpClient();
            string apiUrl = "https://localhost:44373/api/Customer/GetAllCustomers?token=" + token + "&shopifyurl=" + shopifyurl + "";
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody.ToString());
            //Dummy Post Request
            ProductDetailModel model = new ProductDetailModel();
            string apiUrl2 = "https://localhost:44373/api/Customer/SendCustomerData";
            // strong typed instance
            ProductDetailModel pdm = new ProductDetailModel();
            pdm.shopifyurl = shopifyurl;
            pdm.token = token;
            var response2 = client.PostAsJsonAsync(apiUrl2, pdm).Result;
            response2.EnsureSuccessStatusCode();
            string responseBody1 = await response2.Content.ReadAsStringAsync();
            List<Customer> customers2 = JsonConvert.DeserializeObject<List<Customer>>(responseBody1.ToString());
            return View();
        }
    }
}