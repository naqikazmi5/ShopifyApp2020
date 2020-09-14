using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopifyApp.Models.OrderHelper;
using ShopifySharp;
using ShopifySharp.Filters;

namespace ShopifyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        //create order in shopify
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody]OrderDetailModel model)
        {
            var response = "";
            try
            {
                //model.token = new HomeController().ReturnToken();
                //var service1 = new CarrierService(model.shopifyurl, model.token);
                var service = new OrderService(model.shopifyurl, model.token);
                var order = new Order()
                {
                    CreatedAt = DateTime.UtcNow,
                    BillingAddress = new Address()
                    {
                        Address1 = "123 4th Street",
                        City = "Minneapolis",
                        Province = "Minnesota",
                        ProvinceCode = "MN",
                        Zip = "55401",
                        Phone = "555-555-5555",
                        FirstName = "John",
                        LastName = "Doe",
                        Company = "Tomorrow Corporation",
                        Country = "United States",
                        CountryCode = "US",
                        Default = true,
                    },
                    LineItems = new List<LineItem>()
    {
        new LineItem()
        {
            Name = "Test Line Item",
            Title = "Test Line Item Title",
            Price = 20
        }
    },
                    FinancialStatus = "paid",
                    TotalPrice = 25,
                    Email = Guid.NewGuid().ToString() + "@example.com",
                    Note = "Test note about the customer.",
                };

                order = await service.CreateAsync(order);
                response = "Order is created successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Update order
        [Route("[action]")]
        public async Task<IActionResult> UpdateOrder([FromBody]OrderDetailModel model)
        {
            var response = "";
            try
            {
                long oid = Convert.ToInt64(model.orderId);
                //model.token = new HomeController().ReturnToken();
                var service = new OrderService(model.shopifyurl, model.token);
                var order = await service.UpdateAsync(oid, new Order()
                {
                    CreatedAt = DateTime.UtcNow,
                    BillingAddress = new Address()
                    {
                        Address1 = "123 4th Street",
                        City = "Minneapolis",
                        Province = "Minnesota",
                        ProvinceCode = "MN",
                        Zip = "55401",
                        Phone = "555-555-5555",
                        FirstName = "John",
                        LastName = "Doe",
                        Company = "Tomorrow Corporation",
                        Country = "United States",
                        CountryCode = "US",
                        Default = true,
                    },
                    LineItems = new List<LineItem>()
    {
        new LineItem()
        {
            Name = "Test Line Item",
            Title = "Test Line Item Title"
        }
    },
                    FinancialStatus = "paid",
                    TotalPrice = 5,
                    Email = Guid.NewGuid().ToString() + "@example.com",
                    Note = "Test note about the customer.",
            });
                response = "Order is updated successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(response);
        }
        //Get order By Id
        [Route("[action]")]
        public async Task<IActionResult> GetOrder([FromBody]OrderDetailModel model)
        {
            dynamic response;
            try
            {
                //model.token = new HomeController().ReturnToken();
                long oid = Convert.ToInt64(model.orderId);
                var service = new OrderService(model.shopifyurl, model.token);
                response = await service.GetAsync(oid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Delete order By Id
        [Route("[action]")]
        public async Task<IActionResult> DeleteOrder([FromBody]OrderDetailModel model)
        {
            var response = "";
            try
            {
                //model.token = new HomeController().ReturnToken();
                long oid = Convert.ToInt64(model.orderId);
                var service = new OrderService(model.shopifyurl, model.token);
                await service.DeleteAsync(oid);
                response = "Order is deleted successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Get All order
        [Route("[action]")]
        public async Task<IActionResult> GetAllOrders(string token, string shopifyurl)
        {
            dynamic response = null;
            try
            {
                //token = new HomeController().ReturnToken();
                var allOrders = new List<Order>();
                var service = new OrderService(shopifyurl,token);
                var page = await service.ListAsync(new OrderListFilter
                {
                    Limit = 250,
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
                response = allOrders;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendOrderData([FromBody]Order model)
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
    }
}