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
            var topCategories = data
                .Select(p => new { Product = p.Key, p.Key.CategoryId, Count = p.Value })
                .GroupBy(p => p.CategoryId)
                .Select(p => new { CategoryId = p.Key, Count = p.Sum(s => s.Count) })
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
    }
}