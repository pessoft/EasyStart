using EasyStart.Models;
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
        
        public ProductService(IDefaultEntityRepository<ProductModel> repository)
        {
            this.repository = repository;
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

            productIds = productIds.Distinct().ToList();
            var products = repository.Get(p => productIds.Contains(p.Id));

            return products.ToList();
        }
    }
}