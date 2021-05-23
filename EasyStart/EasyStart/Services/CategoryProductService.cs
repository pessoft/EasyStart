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
        private readonly IBranchLogic branchLogic;

        public CategoryProductService(
            ICategoryProductLogic categoryProductLogic,
            IBranchLogic branchLogic)
        {
            this.categoryProductLogic = categoryProductLogic;
            this.branchLogic = branchLogic;
        }

        public CategoryModel Get(int id)
        {
            return categoryProductLogic.Get(id);
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
            return categoryProductLogic.RemoveCategory(id);
        }
    }
}