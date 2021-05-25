using EasyStart.Models;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.CategoryProduct
{
    public interface ICategoryProductLogic: IBranchRemoval
    {
        CategoryModel Get(int id);
        List<CategoryModel> GetByBranch(int branchId);
        CategoryModel SaveCategory(CategoryModel category);
        bool RemoveCategory(int id);
    }
}
