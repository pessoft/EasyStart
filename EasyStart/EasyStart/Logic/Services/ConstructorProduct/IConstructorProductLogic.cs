using EasyStart.Models;
using System.Collections.Generic;

namespace EasyStart.Logic.Services.ConstructorProduct
{
    public interface IConstructorProductLogic : IBranchRemoval
    {
        void UpdateOrder(List<UpdaterOrderNumber> items);
        IEnumerable<IngredientModel> GetIngredientsByCategoryIds(IEnumerable<int> categoryIds);
        IEnumerable<IngredientModel> GetIngredientsByConstructorCategoryIds(IEnumerable<int> constructorCategoryIds);
        void RemoveConstructorCategory(int id);
        void RemoveIngredientsByCategoryConstructorId(int categoryConstructorId);

        ProductConstructorIngredientModel AddOrUpdateCategoryConstructor(ProductConstructorIngredientModel category);
        IEnumerable<ProductConstructorIngredientModel> GetConstructorProducts(int categoryId);
    }
}
