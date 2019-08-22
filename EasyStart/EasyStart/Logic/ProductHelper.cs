using EasyStart.Models;
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
                Name = String.Copy(category.Name),
                OrderNumber = category.OrderNumber,
                Visible = category.Visible
            };
        }

        public static ProductModel Clone(this ProductModel product, int newBrachId, int newCategoryId, string newImageName)
        {
            return new ProductModel
            {
                Id = -1,
                BranchId = newBrachId,
                Image = newImageName,
                AdditionInfo = String.Copy(product.AdditionInfo),
                CategoryId = newCategoryId,
                Description = String.Copy(product.Description),
                Name = String.Copy(product.Name),
                OrderNumber = product.OrderNumber,
                Price = product.Price,
                ProductType = product.ProductType,
                Rating = product.Rating,
                Visible = product.Visible,
                VotesCount = product.VotesCount,
                VotesSum = product.VotesSum
            };
        }
    }
}