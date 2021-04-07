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
        private readonly IDefaultEntityRepository<ProductModel> repository;
        private readonly IDefaultEntityRepository<AdditionalFilling> additionalFillingRepository;
        private readonly IDefaultEntityRepository<AdditionOptionItem> additionOptionItemRepository;
        private readonly IDefaultEntityRepository<ProductAdditionalFillingModal> productAdditionalFillingRepository;
        private readonly IDefaultEntityRepository<ProductAdditionalOptionModal> productAdditionOptionItemRepository;

        public ProductService(
            IDefaultEntityRepository<ProductModel> repository,
            IDefaultEntityRepository<AdditionalFilling> additionalFillingRepository,
            IDefaultEntityRepository<AdditionOptionItem> additionOptionItemRepository,
            IDefaultEntityRepository<ProductAdditionalFillingModal> productAdditionalFillingRepository,
            IDefaultEntityRepository<ProductAdditionalOptionModal> productAdditionOptionItemRepository)
        {
            this.repository = repository;
            this.additionalFillingRepository = additionalFillingRepository;
            this.additionOptionItemRepository = additionOptionItemRepository;
            this.productAdditionalFillingRepository = productAdditionalFillingRepository;
            this.productAdditionOptionItemRepository = productAdditionOptionItemRepository;
        }

        public ProductModel Get(int id)
        {
            var result = repository.Get(id);

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
            var products = repository.Get(p => productIds.Contains(p.Id)).ToList();

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
    }
}