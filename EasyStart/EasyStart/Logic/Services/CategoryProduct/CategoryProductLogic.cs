using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace EasyStart.Logic.Services.Product
{
    public class CategoryProductLogic : CatalogItemBase, ICategoryProductLogic
    {
        private readonly IBaseRepository<CategoryModel, int> categoryRepository;
        private readonly IBaseRepository<RecommendedProductModel, int> recommendedProductRepository;

        public CategoryProductLogic(
            IBaseRepository<CategoryModel, int> categoryRepository,
            IBaseRepository<RecommendedProductModel, int> recommendedProductRepository)
        {
            this.categoryRepository = categoryRepository;
            this.recommendedProductRepository = recommendedProductRepository;
        }

        public CategoryModel Get(int id)
        {
            return categoryRepository.Get(id);
        }

        public bool RemoveCategory(int id)
        {
            var category = Get(id);

            if (category == null)
                return false;

            category.IsDeleted = true;
            categoryRepository.Update(category);

            return true;
        }

        public CategoryModel SaveCategory(CategoryModel category)
        {
            var oldCategory = categoryRepository.Get(category.Id);
            CategoryModel savedCategory = null;

            PrepareImage(category);

            if (oldCategory != null)
            {
                category.OrderNumber = oldCategory.OrderNumber;

                RemoveOldImage(oldCategory, category);
                savedCategory = categoryRepository.Update(category);
            }
            else
            {
                var orderNumber = categoryRepository.Get(p => p.BranchId == category.BranchId && !p.IsDeleted).Count() + 1;
                category.OrderNumber = orderNumber;
                
                savedCategory = categoryRepository.Create(category);
            }
                

            var recommendedProducts = SaveRecommendedProductsForCategory(
                category.RecommendedProducts,
                savedCategory.BranchId,
                savedCategory.Id);
            savedCategory.RecommendedProducts = recommendedProducts;

            return savedCategory;
        }

        private List<int> SaveRecommendedProductsForCategory(
            List<int> recommendedProducts,
            int branchId,
            int categoryId)
        {
            List<RecommendedProductModel> recommendedProductModels = recommendedProducts?
                .Select(productId => new RecommendedProductModel
                {
                    BranchId = branchId,
                    CategoryId = categoryId,
                    ProductId = productId
                })
                .ToList();

            var forRemove = recommendedProductRepository.Get(p => p.CategoryId == categoryId).ToList();
            recommendedProductRepository.Remove(forRemove);

            if (recommendedProductModels != null && recommendedProductModels.Any())
            {
                recommendedProductRepository.Create(recommendedProductModels);
            }

            return recommendedProducts;
        }
    }
}