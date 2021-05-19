using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class RecommendedProductModel: BaseEntity
    {
        public int CategoryId { get; set; }
        public int BranchId { get; set; }
        public int ProductId { get; set; }
    }
}