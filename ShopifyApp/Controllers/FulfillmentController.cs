using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopifyApp.Models.FulfillmentHelper;
using ShopifySharp;

namespace ShopifyApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FulfillmentController : ControllerBase
    {
        //create order fulfillment in shopify
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateFulfillment([FromBody] FulfillmentDetailModel model)
        {
            var response = "";
            try
            {
                string shopifyurl = $"https://{model.shopifyurl}.myshopify.com";
                var locationservice = new LocationService(shopifyurl, model.token);
                var locations = await locationservice.ListAsync();
                var lid = locations.Where(x => x.Name.ToLower() == "parco").FirstOrDefault();
                var service = new FulfillmentService(shopifyurl, model.token);
                var fulfillment = new Fulfillment()
                {
                    TrackingCompany = model.trackingCompany,
                    TrackingUrl = model.trackingUrl,
                    TrackingNumber = model.trackingNumber,
                    LocationId = lid.Id
                };
                fulfillment = await service.CreateAsync(model.orderId, fulfillment);
                response = "success";
            }
            catch (Exception ex)
            {
               // response = ex.InnerException?.Message.ToString();
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //create  order partial fulfillment in shopify
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreatePartialFulfillment([FromBody] FulfillmentDetailModel model)
        {
            var response = "";
            try
            {
                var service = new FulfillmentService(model.shopifyurl, model.token);
                var fulfillment = new Fulfillment()
                {
                    TrackingCompany = "Jack Black's Pack, Stack and Track",
                    TrackingUrl = "https://example.com/123456789",
                    TrackingNumber = "123456789",
                    //LineItems = new List<LineItem>()
                    //{
                    //new LineItem()
                    //{
                    //Id = lineItemId,
                    //Quantity = 1 //Fulfills 1 qty of this line item.
                    //}
                    //}
                    };

                fulfillment = await service.CreateAsync(model.orderId, fulfillment);
                response = "Partial Fulfillment is created successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //create  order partial fulfillment in shopify
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CreateSingleFulfillment([FromBody] FulfillmentDetailModel model)
        {
            var response = "";
            try
            {
                var service = new FulfillmentService(model.shopifyurl, model.token);
                var fulfillment = new Fulfillment()
                {
                    TrackingCompany = "Jack Black's Pack, Stack and Track",
                    TrackingUrl = "https://example.com/123456789",
                    TrackingNumber = "123456789",
    //                LineItems = new List<LineItem>()
    //{
    //    new LineItem()
    //    {
    //        Id = lineItemId
    //    }
    //}
                };

                fulfillment = await service.CreateAsync(model.orderId, fulfillment);
                response = "Single Fulfillment is created successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Retrieving a fulfillment
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> RetrievingFulfillment([FromBody] FulfillmentDetailModel model)
        {
            Fulfillment response = new Fulfillment();
            try
            {
                var service = new FulfillmentService(model.shopifyurl, model.token);
                 response = await service.GetAsync(model.orderId, model.fulfillmentId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }

        //Updating a fulfillment
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UpdatingFulfillment([FromBody] FulfillmentDetailModel model)
        {
            Fulfillment response = new Fulfillment();
            try
            {
                var service = new FulfillmentService(model.shopifyurl, model.token);
                response = await service.UpdateAsync(model.orderId, model.fulfillmentId, new Fulfillment()
                {
                    TrackingCompany = "John Doe's Tracking Company"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Completing a fulfillment
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CompletingFulfillment([FromBody] FulfillmentDetailModel model)
        {
            var response = "";
            try
            {
                var service = new FulfillmentService(model.shopifyurl, model.token);
                await service.CompleteAsync(model.orderId, model.fulfillmentId);
                response = "Fulfillment is completed successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
        //Cancelling a fulfillment
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> CancellingFulfillment([FromBody] FulfillmentDetailModel model)
        {
            var response = "";
            try
            {
                var service = new FulfillmentService(model.shopifyurl, model.token);
                await service.CancelAsync(model.orderId, model.fulfillmentId);
                response = "Fulfillment is cancelled successfuly!";
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(response);
        }
    }
}
