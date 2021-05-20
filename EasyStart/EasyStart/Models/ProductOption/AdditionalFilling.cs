using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.ProductOption
{
    public class AdditionalFilling: BaseEntity<int>
    {
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string VendorCode { get; set; }
        public double Price { get; set; }
        public bool IsDeleted { get; set; }
    }
}