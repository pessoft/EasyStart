using EasyStart.Logic.Services.Branch;
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
    public class ProductService
    {
        private readonly IProductLogic productLogic;
        private readonly IBranchLogic branchLogic;

        public ProductService(
            IProductLogic productLogic,
            IBranchLogic branchLogic)
        {
            this.productLogic = productLogic;
            this.branchLogic = branchLogic;
        }

        public ProductModel Get(int id)
        {
            return productLogic.Get(id);
        }

        /// <summary>
        /// Возвращает продукты на основе ProductCount и ProductBonusCount
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public List<ProductModel> Get(OrderModel order)
        {
            return productLogic.Get(order);
        }

        public List<AdditionalFilling> GetAdditionalFillingsByBranchId(int branchId)
        {
            return productLogic.GetAdditionalFillingsByBranchId(branchId);
        }

        public List<AdditionOptionItem> GetAdditionOptionItemByBranchId(int branchId)
        {
            return productLogic.GetAdditionOptionItemByBranchId(branchId);
        }

        public Dictionary<int, List<ProductAdditionalFillingModal>> GetProductAdditionalFillingsByProductIds(List<int> productIds)
        {
            return productLogic.GetProductAdditionalFillingsByProductIds(productIds);
        }

        public Dictionary<int, List<ProductAdditionalOptionModal>> GetProductAdditionOptionItemByProductIds(List<int> productIds)
        {
            return productLogic.GetProductAdditionOptionItemByProductIds(productIds);
        }

        public ProductModel SaveProduct(ProductModel product)
        {
            var branch = branchLogic.Get();
            product.BranchId = branch.Id;

            return productLogic.SaveProduct(product);
        }
    }
}