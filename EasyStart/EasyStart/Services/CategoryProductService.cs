using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.CategoryProduct;
using EasyStart.Logic.Services.Product;
using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class CategoryProductService
    {
        private readonly ICategoryProductLogic categoryProductLogic;
        private readonly IProductLogic productLogic;
        private readonly IBranchLogic branchLogic;

        public CategoryProductService(
            ICategoryProductLogic categoryProductLogic,
            IProductLogic productLogic,
            IBranchLogic branchLogic)
        {
            this.categoryProductLogic = categoryProductLogic;
            this.productLogic = productLogic;
            this.branchLogic = branchLogic;
        }

        public CategoryModel Get(int id)
        {
            return categoryProductLogic.Get(id);
        }

        public List<CategoryModel> GetByBranch()
        {
            var branch = branchLogic.Get();

            return categoryProductLogic.GetByBranch(branch.Id);
        }

        public CategoryModel SaveCategory(CategoryModel category)
        {
            var branch = branchLogic.Get();
            category.BranchId = branch.Id;

            var savedCategory = categoryProductLogic.SaveCategory(category);

            return savedCategory;
        }

        public bool RemoveCategory(int id)
        {
            var success = categoryProductLogic.RemoveCategory(id);

            if (success)
                productLogic.RemoveProductByCategory(id);

            return success;
        }

        public void UpdateOrder(List<UpdaterOrderNumber> items)
        {
            categoryProductLogic.UpdateOrder(items);
        }

        public void UpdateVisible(UpdaterVisible update)
        {
            categoryProductLogic.UpdateVisible(update);
        }
    }
}