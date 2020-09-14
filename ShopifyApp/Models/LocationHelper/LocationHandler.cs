using ShopifySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopifyApp.Models.LocationHelper
{
    public class LocationHandler
    {
        public async Task<List<LocationReturnModel>> GetLocationReturnModel(IEnumerable<Location> locations, string shopname)
        {
            List<LocationReturnModel> locationReturns = new List<LocationReturnModel>();
            foreach (var item in locations)
            {
                LocationReturnModel returnModel = new LocationReturnModel
                {
                    Active = item.Active,
                    Address1 = item.Address1,
                    Address2 = item.Address2,
                    AdminGraphQLAPIId = item.AdminGraphQLAPIId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    City = item.City,
                    Country = item.Country,
                    CountryCode = item.CountryCode,
                    CountryName = item.CountryName,
                    Id = item.Id,
                    Legacy = item.Legacy,
                    Name = item.Name,
                    Phone = item.Phone,
                    Province = item.Province,
                    ProvinceCode = item.ProvinceCode,
                    StoreName = shopname,
                    Zip = item.Zip
                };
                locationReturns.Add(returnModel);
            }
            return locationReturns;
        }
    }
}
