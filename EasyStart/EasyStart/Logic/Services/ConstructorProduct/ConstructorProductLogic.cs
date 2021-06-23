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
        private IBaseRepository<ConstructorCategory, int> categoryRepository;
        private IBaseRepository<IngredientModel, int> ingredientRepository;
        private readonly IDisplayItemSettingLogic displayItemSettingLogic;

        public ConstructorProductLogic(
            IBaseRepository<ConstructorCategory, int> categoryRepository,
            IBaseRepository<IngredientModel, int> ingredientRepository,
            IDisplayItemSettingLogic displayItemSettingLogic)
        {
            this.categoryRepository = categoryRepository;
            this.ingredientRepository = ingredientRepository;
            this.displayItemSettingLogic = displayItemSettingLogic;
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

        public void UpdateOrder(List<UpdaterOrderNumber> items)
        {
            displayItemSettingLogic.UpdateOrder(categoryRepository, items);
        }
    }
}