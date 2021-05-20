using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.ProductOption
{
    public class AdditionOptionItem: BaseEntity<int>
    {
        public int BranchId { get; set; }
        public int AdditionOptionId { get; set; }
        public string Name { get; set; }
        public double AdditionalInfo { get; set; }
        public double Price { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
}