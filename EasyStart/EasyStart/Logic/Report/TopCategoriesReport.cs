using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EasyStart.Models;

namespace EasyStart.Logic.Report
{
    public class TopCategoriesReport : TopProductsReport, IReport
    {
        public TopCategoriesReport(ReportFilter filter, int countTop = 5) : base(filter, countTop)
        { }

        public new ReportResult GetReport()
        {
            ReportResult result = null;
            var data = GetData();
            var productConstructorData = GetProductConstructorData();
            var intermediateCategories = data
             .Select(p => new { Product = p.Key, p.Key.CategoryId, Count = p.Value })
             .GroupBy(p => p.CategoryId)
             .Select(p => new TopCategoriesReportModel(p.Key, p.Sum(s => s.Count)))
             .ToList();

            foreach(var item in intermediateCategories)
            {
                var productConstructorCount = 0;

                if (productConstructorData.TryGetValue(item.CategoryId, out productConstructorCount))
                {
                    item.Count += productConstructorCount;
                    productConstructorData.Remove(item.CategoryId);
                }
            }

            if (productConstructorData != null && productConstructorData.Any())
            {
                foreach (var kv in productConstructorData)
                {
                    intermediateCategories.Add(new TopCategoriesReportModel(kv.Key, kv.Value));
                }
            }

            var topCategories = intermediateCategories
                .OrderByDescending(p => p.Count)
                .Take(countTop);

            var categories = DataWrapper.GetCategories(topCategories.Select(p => p.CategoryId));

            if (topCategories != null && topCategories.Any())
            {
                result = new ReportResult();

                foreach (var topCategory in topCategories)
                {
                    var category = categories[topCategory.CategoryId];

                    result.Labels.Add(category.Name);
                    result.Data.Add(topCategory.Count);
                }
            }

            return result;
        }

        /// <summary>
        /// key - Constructor Category Id
        /// value - Count
        /// </summary>
        /// <returns></returns>
        protected Dictionary<int, int> GetProductConstructorData()
        {
            var data = AnalyticsDataWrapper.GetOrders(filter.DateFrom, filter.DateTo, filter.BranchId);
            var dict = data.Where(p => p.ProductConstructorCount != null && p.ProductConstructorCount.Any())
                .SelectMany(p => p.ProductConstructorCount)
                .GroupBy(p => p.CategoryId)
                .ToDictionary(p => p.Key, p => p.Sum(s => s.Count));

            return dict;
        }
    }
}