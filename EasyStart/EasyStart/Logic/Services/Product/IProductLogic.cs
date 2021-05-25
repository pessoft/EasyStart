using EasyStart.Models;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.Product
{
    public interface IProductLogic: IBranchRemoval
    {
        ProductModel Get(int id);
        List<ProductModel> Get(OrderModel order);
        List<ProductModel> GetByCategory(int categoryId);
        List<AdditionalFilling> GetAdditionalFillingsByBranchId(int branchId);
        List<AdditionalOption> GetAdditionalOptionsByBranchId(int branchId);
        List<AdditionOptionItem> GetAdditionOptionItemByBranchId(int branchId);
        Dictionary<int, List<ProductAdditionalFillingModal>> GetProductAdditionalFillingsByProductIds(List<int> productIds);
        Dictionary<int, List<ProductAdditionalOptionModal>> GetProductAdditionOptionItemByProductIds(List<int> productIds);
        ProductModel SaveProduct(ProductModel product);
        void RemoveProductByCategory(int categoryId);
    }
}
