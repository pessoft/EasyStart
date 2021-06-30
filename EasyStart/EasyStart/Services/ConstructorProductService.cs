using EasyStart.Logic.Services.CategoryProduct;
using EasyStart.Logic.Services.ConstructorProduct;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class ConstructorProductService
    {
        private readonly IConstructorProductLogic constructorProductLogic;
        private readonly ICategoryProductLogic categoryProductLogic;

        public ConstructorProductService(
            IConstructorProductLogic constructorProductLogic,
            ICategoryProductLogic categoryProductLogic)
        {
            this.constructorProductLogic = constructorProductLogic;
            this.categoryProductLogic = categoryProductLogic;
        }

        public void UpdateOrder(List<UpdaterOrderNumber> items)
        {
            constructorProductLogic.UpdateOrder(items);
        }

        public IEnumerable<IngredientModel> GetIngredients(IEnumerable<int> categoryIds)
        {
            return constructorProductLogic.GetIngredients(categoryIds);
        }

        public IEnumerable<OrderDetailConstructorProduct> GetConstructorProductOrderDetails(IEnumerable<int> categoryIds)
        {
            var ingredientDict = GetIngredients(categoryIds)
                .GroupBy(p => p.CategoryId)
                .ToDictionary(p => p.Key, p => p.ToList());
            var categoryDict = categoryProductLogic.Get(ingredientDict.Keys).ToDictionary(p => p.Id);

            var constructorProductDetails = ingredientDict.Select(p =>
            {
                var category = categoryDict[p.Key];

                return new OrderDetailConstructorProduct
                {
                    CategoryId = category.Id,
                    CategoryName = category.Name,
                    CategoryImage = category.Image,
                    Ingredients = p.Value,
                };

            });

            return constructorProductDetails;
        }
    }
}