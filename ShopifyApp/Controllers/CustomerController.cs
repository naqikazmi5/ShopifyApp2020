using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopifyApp.Models.CustomerHelper;
using ShopifySharp;
using ShopifySharp.Filters;

namespace ShopifyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        [HttpPost]
        //create customer in shopify
        [Route("[action]")]
        public async Task<IActionResult> CreateCustomer([FromBody]CustomerDetailModel model)
        {
            var response = "";
            try
            {
                //model.token = new HomeController().ReturnToken();
                var service = new CustomerService(model.shopifyurl, model.token);
                for (int i = 0; i < 10; i++)
                {
                    var customer = new Customer()
                    {

                        FirstName = "John",
                        LastName = "Doe",
                        Email = $"john.doe200{i}@example.com",
                        Addresses = new List<Address>()
                    {
        new Address()
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
        }
    },

                        VerifiedEmail = true,
                        Note = "Test note about the customer.",
                        State = "enabled"
                    };
                    customer = await service.CreateAsync(customer);
                }
                
                response = "Customer is added successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Update customer
        [Route("[action]")]
        public async Task<IActionResult> UpdateCustomer([FromBody]CustomerDetailModel model)
        {
            var response = "";
            try
            {
                long cid = Convert.ToInt64(model.customerId);
                //model.token = new HomeController().ReturnToken();
                var service = new CustomerService(model.shopifyurl, model.token);
                var customer = await service.UpdateAsync(cid, new Customer()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Addresses = new List<Address>()
                    {
        new Address()
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
        }
    },

                    VerifiedEmail = true,
                    Note = "Test note about the customer.",
                    State = "enabled"
                });
                response = "Customer is updated successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(response);
        }
        //Get customer By Id
        [Route("[action]")]
        public async Task<IActionResult> GetCustomer([FromBody]CustomerDetailModel model)
        {
            dynamic response;
            try
            {
                //model.token = new HomeController().ReturnToken();
                long cid = Convert.ToInt64(model.customerId);
                var service = new CustomerService(model.shopifyurl, model.token);
                response = await service.GetAsync(cid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Delete customer By Id
        [Route("[action]")]
        public async Task<IActionResult> DeleteCustomer([FromBody]CustomerDetailModel model)
        {
            var response = "";
            try
            {
                //model.token = new HomeController().ReturnToken();
                long cid = Convert.ToInt64(model.customerId);
                var service = new CustomerService(model.shopifyurl, model.token);
                await service.DeleteAsync(cid);
                response = "Customer is deleted successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Get All customers
        [Route("[action]")]
        public async Task<IActionResult> GetAllCustomers(string token, string shopifyurl)
        {
            dynamic response = null;
            try
            {
                //token = new HomeController().ReturnToken();
                var allCustomers = new List<Customer>();
                var service = new CustomerService(shopifyurl, token);
                var page = await service.ListAsync(new CustomerListFilter
                {
                    Limit = 250,
                });

                while (true)
                {
                    allCustomers.AddRange(page.Items);

                    if (!page.HasNextPage)
                    {
                        break;
                    }

                    page = await service.ListAsync(page.GetNextPageFilter());
                }
                response = allCustomers;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SendCustomerData([FromBody]Customer model)
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
        [HttpPost]
        public ActionResult AddCustomer()
        {
            var req = Request.Body;
            var json = new StreamReader(req).ReadToEnd();
            return Ok(200);

        }
    }
}