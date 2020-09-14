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

namespace ShopifyApp.Models.ProductHelper
{
    public class ProductHandler
    {
        static HttpClient client = new HttpClient();
        public async Task<List<Product>> GetAllProducts(ProductDetailModel model)
        {
            Product product = new Product();
            List<Product> Products = new List<Product>();
            string msg;
            try
            {
                var allProducts = new List<Product>();
                var service = new ProductService(model.shopifyurl, model.token);
                var page = await service.ListAsync(new ProductListFilter
                {
                    Limit = 2,
                });
                allProducts.AddRange(page.Items);
                //while (true)
                //{
                //    allProducts.AddRange(page.Items);

                //    if (!page.HasNextPage)
                //    {
                //        break;
                //    }

                //    page = await service.ListAsync(page.GetNextPageFilter());
                //}
                Products = allProducts;
                var mservice = new MetaFieldService(model.shopifyurl, model.token);
                foreach (var item in Products)
                {
                    var metafields = await mservice.ListAsync(Convert.ToInt64(item.Id), "products");
                    string value = JsonConvert.SerializeObject(metafields.Items);
                    item.Metafields = metafields.Items;
                    foreach (var vgt in item.Variants)
                    {
                        var vmetafields = await mservice.ListAsync(Convert.ToInt64(vgt.Id), "variants");
                        vgt.Metafields = vmetafields.Items;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return Products;
        }
        public async Task<Product> GetProduct(ProductDetailModel model)
        {
            Product product = new Product();
            string msg;
            try
            {
                var allProducts = new List<Product>();
                var service = new ProductService(model.shopifyurl, model.token);
                product = await service.GetAsync(Convert.ToInt64(model.productId));
                var mservice = new MetaFieldService(model.shopifyurl, model.token);
                foreach (var vgt in product.Variants)
                {
                    var vmetafields = await mservice.ListAsync(Convert.ToInt64(vgt.Id), "variants");
                    vgt.Metafields = vmetafields.Items;
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message.ToString();
            }
            return product;
        }
        public async Task<List<ProductReturnModel>> GetProductReturnModel(List<Product> products, string shopname)
        {
            List<ProductReturnModel> productReturns = new List<ProductReturnModel>();
            foreach (var item in products)
            {
                ProductReturnModel returnModel = new ProductReturnModel
                {
                    CreatedAt = item.CreatedAt,
                    BodyHtml = item.BodyHtml,
                    PublishedAt = item.PublishedAt,
                    UpdatedAt = item.UpdatedAt,
                    StoreName = shopname,
                    Handle = item.Handle,
                    Id = item.Id,
                    Images = item.Images,
                    Metafields = item.Metafields,
                    PublishedScope = item.PublishedScope,
                    TemplateSuffix = item.TemplateSuffix,
                    Options = item.Options,
                    ProductType = item.ProductType,
                    Tags = item.Tags,
                    Title = item.Title,
                    Variants = item.Variants,
                    Vendor = item.Vendor,
                };
                productReturns.Add(returnModel);
            }
            return productReturns;
        }
        public async Task<List<Product>> ReturnProductModel(List<ProductReturnModel> products)
        {
            List<Product> productReturns = new List<Product>();
            foreach (var item in products)
            {
                Product returnModel = new Product
                {
                    CreatedAt = item.CreatedAt,
                    BodyHtml = item.BodyHtml,
                    PublishedAt = item.PublishedAt,
                    UpdatedAt = item.UpdatedAt,
                    Handle = item.Handle,
                    Id = item.Id,
                    Images = item.Images,
                    Metafields = item.Metafields,
                    PublishedScope = item.PublishedScope,
                    TemplateSuffix = item.TemplateSuffix,
                    Options = item.Options,
                    ProductType = item.ProductType,
                    Tags = item.Tags,
                    Title = item.Title,
                    Variants = item.Variants,
                    Vendor = item.Vendor,
                };
                productReturns.Add(returnModel);
            }
            return productReturns;
        }
        public async Task<string> AddMetaFeilds(List<Product> products, string shopifyurl, string token)
        {
            foreach (var item in products)
            {
                long id = Convert.ToInt64(item.Id);
                var service = new MetaFieldService(shopifyurl, token);
                var count = await service.CountAsync(id, "products");
                var metafields = await service.ListAsync(id, "products");
                foreach (var item1 in metafields.Items)
                {
                    await service.DeleteAsync(Convert.ToInt64(item1.Id));
                }

                var metafield1 = new MetaField()
                {
                    Namespace = "products/update",
                    Key = "myKey",
                    Value = "5",
                    ValueType = "integer",
                    Description = "This is a test meta field. It is an integer value.",
                    OwnerResource = "products/update",
                };
                //Create a new metafield on a product
                metafield1 = await service.CreateAsync(metafield1, id, "products");
            }

            return "";
        }
        public async Task<bool> SyncProduct(string shopname, string token, string serverurl)
        {
            bool response = true;
            try
            {
                string shopifyurl = $"https://{shopname}.myshopify.com/";
                var client = new HttpClient();
                ProductDetailModel model = new ProductDetailModel();
                model.shopifyurl = shopifyurl;
                model.token = token;
                List<Product> products = await GetAllProducts(model);
                List<ProductReturnModel> productReturns = await GetProductReturnModel(products, shopname);
                string apiUrl2 = $"{serverurl}/api/AddShopifyProducts";
                string json = JsonConvert.SerializeObject(productReturns);
                var response2 = client.PostAsJsonAsync(apiUrl2, productReturns).Result;
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
