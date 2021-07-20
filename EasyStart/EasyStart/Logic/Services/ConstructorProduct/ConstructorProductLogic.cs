using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Models;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyStart.Logic.Services.ConstructorProduct
{
    public class ConstructorProductLogic : IConstructorProductLogic
    {
        private IRepository<ConstructorCategory, int> categoryRepository;
        private IRepository<IngredientModel, int> ingredientRepository;
        private readonly IDisplayItemSettingLogic displayItemSettingLogic;
        private readonly IContainImageLogic imageLogic;

        public ConstructorProductLogic(
            IRepositoryFactory repositoryFactory,
            IDisplayItemSettingLogic displayItemSettingLogic,
            IContainImageLogic imageLogic)
        {
            categoryRepository = repositoryFactory.GetRepository<ConstructorCategory, int>();
            ingredientRepository = repositoryFactory.GetRepository<IngredientModel, int>();
            this.displayItemSettingLogic = displayItemSettingLogic;
            this.imageLogic = imageLogic;
        }

        public IEnumerable<IngredientModel> GetIngredients(IEnumerable<int> categoryIds)
        {
            if (categoryIds == null)
                return new List<IngredientModel>();

            return ingredientRepository.Get(p => categoryIds.Contains(p.CategoryId)
            && !p.IsDeleted);
        }

        public void UpdateOrder(List<UpdaterOrderNumber> items)
        {
            displayItemSettingLogic.UpdateOrder(categoryRepository, items);
        }

        public void RemoveByBranch(int branchId)
        {
            var removedSubCatigories = RemoveCategoriesByBranch(branchId);
            RemoveIngredientsBySubCategory(removedSubCatigories);
        }

        private IEnumerable<int> RemoveCategoriesByBranch(int branchId)
        {
            var categories = categoryRepository.Get(p => p.BranchId == branchId).ToList();
            
            categoryRepository.MarkAsDeleted(categories);

            return categories.Select(p => p.Id);
        }

        private void RemoveIngredientsBySubCategory(IEnumerable<int> subCategoryIds)
        {
            var ingredients = ingredientRepository.Get(p => subCategoryIds.Contains(p.SubCategoryId)).ToList();
            
            ingredientRepository.MarkAsDeleted(ingredients);
        }

        public void RemoveConstructorCategory(int id)
        {
            var entity = categoryRepository.Get(id);
            categoryRepository.MarkAsDeleted(entity);

            RecalcConstructorCategoryOrderNumber(entity.CategoryId);
            RemoveIngredientsByCategoryConstructorId(id);
        }

        public void RemoveIngredientsByCategoryConstructorId(int categoryConstructorId)
        {
            var ingredients = ingredientRepository.Get(p =>
                p.SubCategoryId == categoryConstructorId
                && !p.IsDeleted)
                .ToList();
            ingredientRepository.MarkAsDeleted(ingredients);
        }

        public ProductConstructorIngredientModel AddOrUpdateCategoryConstructor(ProductConstructorIngredientModel category)
        {
            if (category.Ingredients == null || !category.Ingredients.Any())
                throw new Exception("Отсутсвтуют ингредиенты");

            imageLogic.PrepareImage(category.Ingredients);

            var constructorCategory = AddOrUpdateConstructorCategory(category);
            var ingredients = AddOrUpdateIngredients(constructorCategory, category.Ingredients);

            return ConvertTotProductConstructorIngredientModel(constructorCategory, ingredients.ToList());
        }

        private ProductConstructorIngredientModel ConvertTotProductConstructorIngredientModel(ConstructorCategory category, List<IngredientModel> ingredients)
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

        private ConstructorCategory AddOrUpdateConstructorCategory(ProductConstructorIngredientModel category)
        {
            var constructorCategory = ConvertToConstructorCategory(category);

            return categoryRepository.CreateOrUpdate(constructorCategory);
        }

        private IEnumerable<IngredientModel> AddOrUpdateIngredients(ConstructorCategory constructorCategory, IEnumerable<IngredientModel> ingredients)
        {
            foreach (var ingredient in ingredients)
            {
                ingredient.SubCategoryId = constructorCategory.Id;
                ingredient.CategoryId = constructorCategory.CategoryId;
            }

            return ingredientRepository.CreateOrUpdate(ingredients);
        }

        private ConstructorCategory ConvertToConstructorCategory(ProductConstructorIngredientModel data)
        {
            return new ConstructorCategory
            {
                Id = data.Id,
                CategoryId = data.CategoryId,
                MinCountIngredient = data.MinCountIngredient,
                MaxCountIngredient = data.MaxCountIngredient,
                Name = data.Name,
                OrderNumber = data.OrderNumber,
                StyleTypeIngredient = data.StyleTypeIngredient,
                BranchId = data.BranchId
            };
        }

        private void RecalcConstructorCategoryOrderNumber(int categoryId)
        {
            var items = categoryRepository
                        .Get(p => p.CategoryId == categoryId && !p.IsDeleted)
                        .OrderBy(p => p.OrderNumber)
                        .ToList();

            for (var i = 0; i < items.Count; ++i)
            {
                items[i].OrderNumber = i + 1;
            }

            categoryRepository.Update(items);
        }
    }
}