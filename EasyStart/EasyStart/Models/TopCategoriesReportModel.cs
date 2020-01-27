using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class TopCategoriesReportModel
    {
        public TopCategoriesReportModel(int categoryId, int count)
        {
            CategoryId = categoryId;
            Count = count;
        }
        public int CategoryId { get; set; }
        public int Count { get; set; }
    }
}