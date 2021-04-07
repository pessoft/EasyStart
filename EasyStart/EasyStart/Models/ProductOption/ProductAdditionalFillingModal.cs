using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.ProductOption
{
    public class ProductAdditionalFillingModal: BaseEntity
    {
        public int BranchId { get; set; }
        public int ProductId { get; set; }
        public int AdditionalFillingId { get; set; }
        public int OrderNumber { get; set; }
        public bool IsDeleted { get; set; }
    }
}