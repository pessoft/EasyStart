using EasyStart.Models;
using System.Collections.Generic;

namespace EasyStart.Logic.Services.ConstructorProduct
{
    public interface IConstructorProductLogic : IBranchRemoval
    {
        void UpdateOrder(List<UpdaterOrderNumber> items);
        IEnumerable<IngredientModel> GetIngredients(IEnumerable<int> categoryIds);
    }
}
