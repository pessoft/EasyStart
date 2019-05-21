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
            var deliverSetting = DataWrapper.GetDeliverySetting(order.BranchId);
            order.Date = DateTime.Now.GetDateTimeNow(deliverSetting.ZoneId);

            var numberOrder = DataWrapper.SaveOrder(order);

            if(numberOrder != -1)
            {
                result.Data = numberOrder;
                result.Success = true;
            }

            return result;
        }

        public List<OrderModel> GetHistoryOrder(int clientId)
        {
            var historyOrder = DataWrapper.GetHistoryOrder(clientId);

            return historyOrder;
        }

        [HttpPost]
        public void UpdateProducRating([FromBody]RatingProducUpdater ratingUp)
        {
            var result = new JsonResultModel();
            var product = DataWrapper.GetProduct(ratingUp.ProductId);
            var votesCount = ++product.VotesCount;
            var score = ratingUp.Score > 0 ? ratingUp.Score : 0;
            var votesSum = product.VotesSum + score;
            var rating = votesSum / votesCount;

            DataWrapper.UpdateRating(ratingUp.ProductId, rating, votesCount, votesSum);
        }

        [HttpPost]
        public void SetProductReviews([FromBody]ProductReview review)
        {
            if (review != null &&
                !string.IsNullOrEmpty(review.PhoneNumber) &&
                !string.IsNullOrEmpty(review.ReviewText) &&
                review.PorudctId > 0)
            {
                review.Date = DateTime.Now;

                DataWrapper.SaveProductReviews(review);
            }
        }

        public List<ProductReview> GetProductReviews(int productId)
        {
            var reviews = DataWrapper.GetProductReviews(productId);

            if(reviews != null && reviews.Any())
            {
                reviews.ForEach(p =>
                {
                    var numberChrar = p.PhoneNumber.ToArray();
                    numberChrar[11] = '*';
                    numberChrar[12] = '*';

                    p.PhoneNumber = string.Join("", numberChrar);
                });
            }

            return reviews;
        }

        public List<StockModel> GetStocks()
        {
            var stocks = DataWrapper.GetStocksVisible();

            return stocks;
        }

        [HttpPost]
        public Client AddOrUpdateClient([FromBody]Client client)
        {
            var newClient = DataWrapper.AddOrUpdateClient(client);

            return newClient;
        }
    }
}