using EasyStart.Logic;
using EasyStart.Models;
using EasyStart.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EasyStart
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AdminAppController : ApiController
    {
        public Dictionary<int, string> GetAllowedCity()
        {
            var alloweCityIds = DataWrapper.GetAllowedCity();
            var cityDictionary = CityHelper.City
                .Where(p => alloweCityIds.Exists(s => s == p.Key))
                .ToDictionary(p => p.Key, p => p.Value);

            return cityDictionary;
        }

        public List<CategoryModel> GetCategories()
        {
            var categories = DataWrapper.GetCategories();

            return categories;
        }

        public List<ProductModel> GetProducts(int categoryId)
        {
            var products = DataWrapper.GetProducts(categoryId);

            return products;
        }

        public Dictionary<int, List<ProductModel>> GetAllProducts()
        {
            var products = DataWrapper.GetAllProducts()
                .GroupBy(p => p.CategoryId)
                .ToDictionary(p => p.Key, p => p.ToList());

            return products;
        }

        public DeliverySettingModel GetDeliverySetting(int cityId)
        {
            var deliverySetting = DataWrapper.GetDeliverySettingByCity(cityId);

            return deliverySetting;
        }

        public SettingModel GetSetting(int cityId)
        {
            var setting = DataWrapper.GetSettingByCity(cityId);

            return setting;
        }

        [HttpPost]
        public JsonResultModel SendOrder([FromBody]OrderModel order)
        {
            var result = new JsonResultModel();
            var numberOrder = DataWrapper.SaveOrder(order);

            if(numberOrder != -1)
            {
                result.Data = numberOrder;
                result.Success = true;
            }

            return result;
        }

        public List<OrderModel> GetHistoryOrder(string phoneNumber)
        {
            //hack - плюс куда то исчезает при передаче
            if(phoneNumber[0] != '+')
            {
                phoneNumber = "+" + phoneNumber.Trim();
            }

            var historyOrder = DataWrapper.GetHistoryOrder(phoneNumber);

            return historyOrder;
        }
    }
}