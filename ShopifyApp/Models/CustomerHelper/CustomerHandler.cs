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

namespace ShopifyApp.Models.CustomerHelper
{
    public class CustomerHandler
    {
        public async Task<List<Customer>> GetAllCustomers(CustomerDetailModel model)
        {
            string msg;
            List<Customer> Customers = new List<Customer>();
            try
            {
                var allCustomers = new List<Customer>();
                var service = new CustomerService(model.shopifyurl, model.token);
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
                Customers = allCustomers;
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return Customers;
        }
        public async Task<List<CustomerReturnModel>> GetCustomerReturnModel(List<Customer> customers, string storename)
        {
            List<CustomerReturnModel> crm = new List<CustomerReturnModel>();
            foreach (var item in customers)
            {
                CustomerReturnModel returnModel = new CustomerReturnModel
                {
                    Addresses = item.Addresses,
                    DefaultAddress = item.DefaultAddress,
                    AcceptsMarketing = item.AcceptsMarketing,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    State = item.State,
                    Id = item.Id,
                    TotalSpent = item.TotalSpent,
                    TaxExempt = item.TaxExempt,
                    Tags = item.Tags,
                    Phone = item.Phone,
                    OrdersCount = item.OrdersCount,
                    Note = item.Note,
                    LastOrderId = item.LastOrderId,
                    LastOrderName = item.LastOrderName,
                    LastName = item.LastName,
                    FirstName = item.FirstName,
                    Email = item.Email,
                    VerifiedEmail = item.VerifiedEmail,
                    StoreName = storename,
                    Currency = item.Currency,
                    Metafields = item.Metafields,
                    MultipassIdentifier = item.MultipassIdentifier
                };
                crm.Add(returnModel);
            }
            return crm;
        }
        public async Task<bool> SyncCustomer(string shopname, string token, string serverurl)
        {
            bool response = true;
            try
            {
                string shopifyurl = $"https://{shopname}.myshopify.com/";
                CustomerDetailModel cdm = new CustomerDetailModel();
                cdm.token = token;
                cdm.shopifyurl = shopifyurl;
                List<Customer> customersList = await GetAllCustomers(cdm);
                List<CustomerReturnModel> customerReturns = await GetCustomerReturnModel(customersList, shopname);
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(customerReturns);
                string apiUrl2 = $"{serverurl}/api/AddShopifyCustomers";
                var response2 = client.PostAsJsonAsync(apiUrl2, customerReturns).Result;
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
