using EasyStart.Models;
using EasyStart.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public static class ProductHelper
    {
        public static CategoryModel Clone(this CategoryModel category, int newBranchId, string newImageName)
        {
            return new CategoryModel
            {
                Id = -1,
                Image = newImageName,
                BranchId = newBranchId,
                Name = category.Name.CloneOrDefault(),
                OrderNumber = category.OrderNumber,
                Visible = category.Visible,
                CategoryType = category.CategoryType,
                NumberAppliances = category.NumberAppliances,
                IsDeleted = category.IsDeleted
            };
        }

        public static ProductModel Clone(
            this ProductModel product,
            int newBrachId,
            int newCategoryId,
            string newImageName,
            Dictionary<int, int> additionalOptionsConformityIds,
            Dictionary<int, int> additionalFillingsConformityIds,
            Dictionary<int, int> additionalOptionItemsConformityIds)
        {
            var allowCombinationsJSON = JsonConvert.SerializeObject(product.AllowCombinations?
                .Select(p => p.Select(x => additionalOptionItemsConformityIds[x]))
                ?? new List<List<int>>());

            return new ProductModel
            {
                Id = -1,
                BranchId = newBrachId,
                Image = newImageName,
                AdditionInfo = product.AdditionInfo != null ? String.Copy(product.AdditionInfo) : null,
                CategoryId = newCategoryId,
                Description = product.Description.CloneOrDefault(),
                Name = product.Name.CloneOrDefault(),
                OrderNumber = product.OrderNumber,
                Price = product.Price,
                ProductType = product.ProductType,
                Rating = product.Rating,
                Visible = product.Visible,
                VotesCount = product.VotesCount,
                VotesSum = product.VotesSum,
                IsDeleted = product.IsDeleted,
                ProductAdditionalInfoType = product.ProductAdditionalInfoType,
                ProductAdditionalOptionIds = product.ProductAdditionalOptionIds.Select(p => additionalOptionsConformityIds[p]).ToList(),
                ProductAdditionalFillingIds = product.ProductAdditionalFillingIds.Select(p => additionalFillingsConformityIds[p]).ToList(),
                AllowCombinationsJSON = allowCombinationsJSON
            };
        }

        public static ConstructorCategory Clone(this ConstructorCategory category, int newBranchId, int newCategoryId)
        {
            return new ConstructorCategory
            {
                Id = -1,
                CategoryId = newCategoryId,
                BranchId = newBranchId,
                Name = category.Name.CloneOrDefault(),
                MinCountIngredient = category.MinCountIngredient,
                MaxCountIngredient = category.MaxCountIngredient,
                StyleTypeIngredient = category.StyleTypeIngredient,
                OrderNumber = category.OrderNumber,
                IsDeleted = category.IsDeleted,
            };
        }

        public static IngredientModel Clone(this IngredientModel ingredients, int newCategoryId, int newSubCategoryId, string newImageName)
        {
            return new IngredientModel
            {
                Id = -1,
                CategoryId = newCategoryId,
                SubCategoryId = newSubCategoryId,
                Name = ingredients.Name.CloneOrDefault(),
                AdditionalInfo = ingredients.AdditionalInfo.CloneOrDefault(),
                Price = ingredients.Price,
                MaxAddCount = ingredients.MaxAddCount,
                Description = ingredients.Description.CloneOrDefault(),
                Image = newImageName,
                IsDeleted = ingredients.IsDeleted
            };
        }
    }
}