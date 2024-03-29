﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyStart.Models;

namespace EasyStart.Logic.Report
{
    public class TopProductsReport : BaseReport, IReport
    {
        protected int countTop;
        public TopProductsReport(ReportFilter filter, int countTop = 5) : base(filter)
        {
            this.countTop = countTop;
        }

        public ReportResult GetReport()
        {
            ReportResult result = null;
            var data = GetData();
            var topProducts = data
                .Select(p => new { Product = p.Key, Count = p.Value })
                .OrderByDescending(p => p.Count)
                .Take(countTop);

            if (topProducts != null && topProducts.Any())
            {
                result = new ReportResult();

                foreach (var topProduct in topProducts)
                {
                    result.Labels.Add(topProduct.Product.Name);
                    result.Data.Add(topProduct.Count);
                }
            }

            return result;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>
        /// value - count product from orders
        /// </returns>
        protected Dictionary<ProductModel, int> GetData()
        {
            var data = AnalyticsDataWrapper.GetOrders(filter.DateFrom, filter.DateTo, filter.BranchId);
            var productIdsFromOrders = data.SelectMany(p => p.ProductCount?.Keys.ToList() ?? new List<int>()).Distinct();
            var productsWithOptions = data.SelectMany(p => p.ProductWithOptionsCount ?? new List<ProductWithOptionsOrderModel>()).ToList();
            var productIdsWithOptionsFromOrders = productsWithOptions.Select(p => p.ProductId).Distinct();
            productIdsFromOrders = productIdsWithOptionsFromOrders != null && productIdsWithOptionsFromOrders.Any()
                ? productIdsFromOrders.Concat(productIdsWithOptionsFromOrders).Distinct()
                : productIdsFromOrders;
            var productsFromOrders = DataWrapper.GetProducts(productIdsFromOrders);
            var productDictionary = productsFromOrders.ToDictionary(p => p.Id, p => p);
            var productCount = data
                .SelectMany(p => p.ProductCount?.ToList() ?? new List<KeyValuePair<int, int>>())
                .GroupBy(p => p.Key)
                .ToDictionary(p => productDictionary[p.Key], p => p.Sum(s => s.Value));

            var productWithOptionsCount = data
                .SelectMany(p => p.ProductWithOptionsCount ?? new List<ProductWithOptionsOrderModel>())
                .GroupBy(p => p.ProductId)
                .ToDictionary(p => productDictionary[p.Key], p => p.Sum(s => s.Count));

            if (productWithOptionsCount.Any())
            {
                productCount = productCount.Concat(productWithOptionsCount).ToDictionary(p => p.Key, p=> p.Value);
            }

            return productCount;
        }
    }
}