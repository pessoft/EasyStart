﻿using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace EasyStart.Logic.Services.CategoryProduct
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
            var category = categoryRepository.Get(id);
            category.RecommendedProducts = recommendedProductRepository.Get(p => p.CategoryId == id).Select(p => p.ProductId).ToList();

            return category; 
        }

        public List<CategoryModel> GetByBranch(int branchId)
        {
            var categories = categoryRepository
                .Get(p => p.BranchId == branchId && !p.IsDeleted)
                .ToList();
            var recommendedProductsDict = recommendedProductRepository
                .Get(p => p.BranchId == branchId)
                .GroupBy(p => p.CategoryId)
                .ToDictionary(
                    p => p.Key,
                    p => p.Select(x => x.ProductId).ToList());
            
            categories.ForEach(p => 
            {
                recommendedProductsDict.TryGetValue(p.Id, out List<int> recommendetProducts);
                p.RecommendedProducts = recommendetProducts;
            });

            return categories;
        }

        public void RemoveByBranch(int branchId)
        {
            var categories = categoryRepository.Get(p => p.BranchId == branchId).ToList();
            categories.ForEach(p => p.IsDeleted = true);

            categoryRepository.Update(categories);
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