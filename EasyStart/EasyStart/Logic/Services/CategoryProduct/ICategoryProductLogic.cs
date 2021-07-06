using EasyStart.Models;
using System.Collections.Generic;

namespace EasyStart.Logic.Services.CategoryProduct
{
    public interface ICategoryProductLogic: IBranchRemoval
    {
        CategoryModel Get(int id);
        IEnumerable<CategoryModel> Get(IEnumerable<int> id);
        List<CategoryModel> GetByBranch(int branchId);
        CategoryModel SaveCategory(CategoryModel category);
        bool RemoveCategory(int id);
        void UpdateOrder(List<UpdaterOrderNumber> items);
        void UpdateVisible(UpdaterVisible update);
    }
}
