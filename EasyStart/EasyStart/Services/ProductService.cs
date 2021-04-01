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

        public ProductService(
            IDefaultEntityRepository<ProductModel> repository,
            IDefaultEntityRepository<AdditionalFilling> additionalFillingRepository,
            IDefaultEntityRepository<AdditionOptionItem> additionOptionItemRepository)
        {
            this.repository = repository;
            this.additionalFillingRepository = additionalFillingRepository;
            this.additionOptionItemRepository = additionOptionItemRepository;
        }

        public ProductModel Get(int id)
        {
            var result = repository.Get(id);

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
            var products = repository.Get(p => productIds.Contains(p.Id));

            return products.ToList();
        }

        public  List<AdditionalFilling> GetAdditionalFillingsByBranchId(int branchId)
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
    }
}