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


        public List<ProductModel> GetProducts(int idCategory)
        {
            var products = DataWrapper.GetProducts(idCategory);

            return products;
        }
    }
}