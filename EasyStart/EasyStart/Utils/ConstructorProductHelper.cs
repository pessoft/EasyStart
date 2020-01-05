using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Utils
{
    public static class ConstructorProductHelper
    {
        public static ConstructorCategory ConvertToConstructorCategory(this ProductConstructorIngredientModel data)
        {
            return new ConstructorCategory
            {
                Id = data.Id,
                CategoryId = data.CategoryId,
                MaxCountIngredient = data.MaxCountIngredient,
                Name = data.Name,
                OrderNumber = data.OrderNumber,
                StyleTypeIngredient = data.StyleTypeIngredient
            };
        }

        public static void UpdateIngredientSubCategoryId(this List<IngredientModel> data, int id)
        {
            if(data != null && data.Any())
            {
                data.ForEach(p => p.SubCategoryId = id);
            }
        }

        public static ProductConstructorIngredientModel GetProductConstructorIngredient(ConstructorCategory category, List<IngredientModel> ingredients)
        {
            return new ProductConstructorIngredientModel
            {
                Id = category.Id,
                CategoryId = category.CategoryId,
                MaxCountIngredient = category.MaxCountIngredient,
                Name = category.Name,
                OrderNumber = category.OrderNumber,
                StyleTypeIngredient = category.StyleTypeIngredient,
                Ingredients = ingredients
            };
        }
    }
}