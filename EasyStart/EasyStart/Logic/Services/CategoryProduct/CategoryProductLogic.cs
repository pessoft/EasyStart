using EasyStart.Logic.Services.Utils;
using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Product
{
    public class CategoryProductLogic : ICategoryProductLogic
    {
        private readonly IDefaultEntityRepository<CategoryModel> categoryRepository;
        private readonly IDefaultEntityRepository<RecommendedProductModel> recommendedProductRepository;
        private readonly IServerUtility serverUtility;

        public CategoryProductLogic(
            IDefaultEntityRepository<CategoryModel> categoryRepository,
            IDefaultEntityRepository<RecommendedProductModel> recommendedProductRepository,
            IServerUtility serverUtility)
        {
            this.categoryRepository = categoryRepository;
            this.recommendedProductRepository = recommendedProductRepository;
            this.serverUtility = serverUtility;
        }

        public CategoryModel Get(int id)
        {
            return categoryRepository.Get(id);
        }

        public CategoryModel SaveCategory(CategoryModel category)
        {
            var defaultImage = "../Images/default-image.jpg";
            var oldCategory = categoryRepository.Get(category.Id);
            CategoryModel savedCategory = null;

            if (!System.IO.File.Exists(serverUtility.MapPath(category.Image)))
                category.Image = defaultImage;

            if (oldCategory != null)
            {
                var oldImage = oldCategory.Image;

                if (oldImage != category.Image
                    && oldImage != defaultImage
                   && System.IO.File.Exists(serverUtility.MapPath(oldImage)))
                {
                    System.IO.File.Delete(serverUtility.MapPath(oldImage));
                }

                savedCategory = categoryRepository.Update(category);
            }
            else
                savedCategory = categoryRepository.Create(category);

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