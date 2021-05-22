using EasyStart.Models;
using EasyStart.Models.ProductOption;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Product
{
    public class ProductLogic: CatalogItemBase, IProductLogic
    {
        private readonly IBaseRepository<ProductModel, int> productRepository;
        private readonly IBaseRepository<AdditionalFilling, int> additionalFillingRepository;
        private readonly IBaseRepository<AdditionOptionItem, int> additionOptionItemRepository;
        private readonly IBaseRepository<ProductAdditionalFillingModal, int> productAdditionalFillingRepository;
        private readonly IBaseRepository<ProductAdditionalOptionModal, int> productAdditionOptionItemRepository;

        public ProductLogic(
            IBaseRepository<ProductModel, int> productRepository,
            IBaseRepository<AdditionalFilling, int> additionalFillingRepository,
            IBaseRepository<AdditionOptionItem, int> additionOptionItemRepository,
            IBaseRepository<ProductAdditionalFillingModal, int> productAdditionalFillingRepository,
            IBaseRepository<ProductAdditionalOptionModal, int> productAdditionOptionItemRepository)
        {
            this.productRepository = productRepository;
            this.additionalFillingRepository = additionalFillingRepository;
            this.additionOptionItemRepository = additionOptionItemRepository;
            this.productAdditionalFillingRepository = productAdditionalFillingRepository;
            this.productAdditionOptionItemRepository = productAdditionOptionItemRepository;
        }

        public ProductModel Get(int id)
        {
            var result = productRepository.Get(id);

            if (result != null)
            {
                var productIds = new List<int> { id };
                var additionalFillings = GetProductAdditionalFillingsByProductIds(productIds);
                var additionalOptions = GetProductAdditionOptionItemByProductIds(productIds);

                result.ProductAdditionalFillingIds = additionalFillings[id].Select(p => p.AdditionalFillingId).ToList();
                result.ProductAdditionalOptionIds = additionalOptions[id].Select(p => p.AdditionalOptionId).ToList();
            }

            return result;
        }

        /// <summary>
        /// Возвращает продукты на основе ProductCount и ProductBonusCount
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public List<ProductModel> Get(OrderModel order)
        {
            var productIds = new List<int>();

            if (order.ProductCount != null)
                productIds.AddRange(order.ProductCount.Keys);

            if (order.ProductBonusCount != null)
                productIds.AddRange(order.ProductBonusCount.Keys);

            if (order.ProductWithOptionsCount != null)
                productIds.AddRange(order.ProductWithOptionsCount.Select(p => p.ProductId));

            productIds = productIds.Distinct().ToList();
            var products = productRepository.Get(p => productIds.Contains(p.Id)).ToList();

            if (products.Any())
            {
                var additionalFillings = GetProductAdditionalFillingsByProductIds(productIds);
                var additionalOptions = GetProductAdditionOptionItemByProductIds(productIds);

                products.ForEach(p =>
                {
                    p.ProductAdditionalFillingIds = additionalFillings[p.Id].Select(x => x.AdditionalFillingId).ToList();
                    p.ProductAdditionalOptionIds = additionalOptions[p.Id].Select(x => x.AdditionalOptionId).ToList();
                });

            }
            return products;
        }

        public List<AdditionalFilling> GetAdditionalFillingsByBranchId(int branchId)
        {
            var additionalfillings = additionalFillingRepository
                .Get(p => p.BranchId == branchId && !p.IsDeleted)
                .ToList();

            return additionalfillings;
        }

        public List<AdditionOptionItem> GetAdditionOptionItemByBranchId(int branchId)
        {
            var additionOptionItems = additionOptionItemRepository
                .Get(p => p.BranchId == branchId && !p.IsDeleted)
                .ToList();

            return additionOptionItems;
        }

        public Dictionary<int, List<ProductAdditionalFillingModal>> GetProductAdditionalFillingsByProductIds(List<int> productIds)
        {
            var additionalfillings = productAdditionalFillingRepository
                .Get(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                .GroupBy(p => p.ProductId)
                .ToDictionary(p => p.Key, p => p.ToList());

            foreach (var productId in productIds)
            {
                if (!additionalfillings.TryGetValue(productId, out List<ProductAdditionalFillingModal> fillings))
                    additionalfillings.Add(productId, new List<ProductAdditionalFillingModal>());
            }

            return additionalfillings;
        }

        public Dictionary<int, List<ProductAdditionalOptionModal>> GetProductAdditionOptionItemByProductIds(List<int> productIds)
        {
            var additionOptionItems = productAdditionOptionItemRepository
                .Get(p => productIds.Contains(p.ProductId) && !p.IsDeleted)
                .GroupBy(p => p.ProductId)
                .ToDictionary(p => p.Key, p => p.ToList());

            foreach (var productId in productIds)
            {
                if (!additionOptionItems.TryGetValue(productId, out List<ProductAdditionalOptionModal> options))
                    additionOptionItems.Add(productId, new List<ProductAdditionalOptionModal>());
            }

            return additionOptionItems;
        }

        public ProductModel SaveProduct(ProductModel product)
        {
            var oldProduct = productRepository.Get(product.Id);
            ProductModel savedProduct = null;

            PrepareImage(product);

            if(oldProduct != null)
            {
                product.OrderNumber = oldProduct.OrderNumber;

                RemoveOldImage(oldProduct, product);
                savedProduct = productRepository.Update(product);
            }
            else
            {
                var orderNumber = productRepository.Get(p => p.CategoryId == product.CategoryId && !p.IsDeleted).Count() + 1;
                product.OrderNumber = orderNumber;
                
                savedProduct = productRepository.Create(product);
            }

            savedProduct.ProductAdditionalOptionIds = product.ProductAdditionalOptionIds;
            SaveProductAdditionalOptions(savedProduct);

            savedProduct.ProductAdditionalFillingIds = product.ProductAdditionalFillingIds;
            SaveProductAdditionalFilling(savedProduct);

            return savedProduct;
        }

        private void SaveProductAdditionalOptions(ProductModel product)
        {
            var orderNumber = 1;
            var additionalOptions = product.ProductAdditionalOptionIds == null ? null : product.ProductAdditionalOptionIds
                .Select(p => new ProductAdditionalOptionModal
                {
                    AdditionalOptionId = p,
                    BranchId = product.BranchId,
                    OrderNumber = orderNumber++,
                    ProductId = product.Id
                }).ToList();
            var oldItems = productAdditionOptionItemRepository
                .Get(p => p.ProductId == product.Id && !p.IsDeleted)
                .ToList();
            oldItems.ForEach(p => p.IsDeleted = true);

            productAdditionOptionItemRepository.Update(oldItems);
            productAdditionOptionItemRepository.Create(additionalOptions);
        }

        private void SaveProductAdditionalFilling(ProductModel product)
        {
            var orderNumber = 1;
            var additionalFillings = product.ProductAdditionalFillingIds == null ? null : product.ProductAdditionalFillingIds
                .Select(p => new ProductAdditionalFillingModal
                {
                    AdditionalFillingId = p,
                    BranchId = product.BranchId,
                    OrderNumber = orderNumber++,
                    ProductId = product.Id
                })
                .ToList();

            var oldItems = productAdditionalFillingRepository
                .Get(p => p.ProductId == product.Id && !p.IsDeleted)
                .ToList();
            oldItems.ForEach(p => p.IsDeleted = true);

            productAdditionalFillingRepository.Update(oldItems);
            productAdditionalFillingRepository.Create(additionalFillings);
        }
    }
}