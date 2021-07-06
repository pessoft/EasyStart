using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.CategoryProduct;
using EasyStart.Logic.Services.Product;
using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repository;
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
        private readonly ICategoryProductLogic categoryProductLogic;

        public ProductService(
            IProductLogic productLogic,
            IBranchLogic branchLogic,
            ICategoryProductLogic categoryProductLogic)
        {
            this.productLogic = productLogic;
            this.branchLogic = branchLogic;
            this.categoryProductLogic = categoryProductLogic;
        }

        public ProductModel Get(int id)
        {
            return productLogic.Get(id);
        }

        public IEnumerable<ProductModel> Get(IEnumerable<int> ids)
        {
            return productLogic.Get(ids);
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

        public List<ProductModel> GetByCategory(int categoryId)
        {
            return productLogic.GetByCategory(categoryId);
        }

        public List<AdditionalFilling> GetAdditionalFillingsByBranch()
        {
            var branch = branchLogic.Get();

            return productLogic.GetAdditionalFillingsByBranchId(branch.Id);
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

        public List<AdditionalOption> GetAdditionalOptionsByBranch()
        {
            var branch = branchLogic.Get();

            return productLogic.GetAdditionalOptionsByBranchId(branch.Id);
        }

        public void RemoveProduct(int id)
        {
            productLogic.RemoveProduct(id);
        }

        public void UpdateOrder(List<UpdaterOrderNumber> items)
        {
            productLogic.UpdateOrder(items);
        }

        public void UpdateVisible(UpdaterVisible update)
        {
            productLogic.UpdateVisible(update);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids">product ids</param>
        /// <returns></returns>
        public IEnumerable<OrderDetailProduct> GetProductOrderDetails(IEnumerable<int> ids)
        {
            var products = Get(ids);

            var idsDict = products
                        .Select(p => p.CategoryId)
                        .Distinct();
            var categoryDict = categoryProductLogic.Get(idsDict).ToDictionary(p => p.Id);
            var productDetails = products
                .GroupBy(p => p.CategoryId)
                .Select(p => new OrderDetailProduct
                {
                    CategoryId = p.Key,
                    CategoryName = categoryDict[p.Key].Name,
                    Products = p.ToList()
                })
                .ToList();

            return productDetails;
        }
    }
}