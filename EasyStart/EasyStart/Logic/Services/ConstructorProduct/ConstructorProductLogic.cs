using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace EasyStart.Logic.Services.ConstructorProduct
{
    public class ConstructorProductLogic : IConstructorProductLogic
    {
        private IRepository<ConstructorCategory, int> categoryRepository;
        private IRepository<IngredientModel, int> ingredientRepository;
        private readonly IDisplayItemSettingLogic displayItemSettingLogic;

        public ConstructorProductLogic(
            IRepository<ConstructorCategory, int> categoryRepository,
            IRepository<IngredientModel, int> ingredientRepository,
            IDisplayItemSettingLogic displayItemSettingLogic)
        {
            this.categoryRepository = categoryRepository;
            this.ingredientRepository = ingredientRepository;
            this.displayItemSettingLogic = displayItemSettingLogic;
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
            
            categories.ForEach(p => p.IsDeleted = true);
            categoryRepository.Update(categories);

            return categories.Select(p => p.Id);
        }

        private void RemoveIngredientsBySubCategory(IEnumerable<int> subCategoryIds)
        {
            var ingredients = ingredientRepository.Get(p => subCategoryIds.Contains(p.SubCategoryId)).ToList();
            
            ingredients.ForEach(p => p.IsDeleted = true);
            ingredientRepository.Update(ingredients);
        }
    }
}