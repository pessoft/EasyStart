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
                MinCountIngredient = data.MinCountIngredient,
                MaxCountIngredient = data.MaxCountIngredient,
                Name = data.Name,
                OrderNumber = data.OrderNumber,
                StyleTypeIngredient = data.StyleTypeIngredient
            };
        }

        /// <summary>
        /// Update category id
        /// Update subcategory id
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        public static void UpdateIngredientSubAndCategoryId(this List<IngredientModel> data, int subCategoryId, int categoryId)
        {
            if(data != null && data.Any())
            {
                data.ForEach(p => { p.SubCategoryId = subCategoryId; p.CategoryId = categoryId; });
            }
        }

        public static ProductConstructorIngredientModel GetProductConstructorIngredient(ConstructorCategory category, List<IngredientModel> ingredients)
        {
            return new ProductConstructorIngredientModel
            {
                Id = category.Id,
                CategoryId = category.CategoryId,
                MinCountIngredient = category.MinCountIngredient,
                MaxCountIngredient = category.MaxCountIngredient,
                Name = category.Name,
                OrderNumber = category.OrderNumber,
                StyleTypeIngredient = category.StyleTypeIngredient,
                Ingredients = ingredients
            };
        }
    }
}