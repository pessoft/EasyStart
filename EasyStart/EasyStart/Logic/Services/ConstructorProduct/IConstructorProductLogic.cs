using EasyStart.Models;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.ConstructorProduct
{
    public interface IConstructorProductLogic : IBranchRemoval
    {
        void UpdateOrder(List<UpdaterOrderNumber> items);
        IEnumerable<IngredientModel> GetIngredients(IEnumerable<int> categoryIds);
    }
}
