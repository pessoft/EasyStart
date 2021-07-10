using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models.ProductOption
{
    public class ProductAdditionalOptionModal: IBaseEntity<int>, IEntityMarkAsDeleted<int>
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ProductId { get; set; }
        public int AdditionalOptionId { get; set; }
        public int OrderNumber { get; set; }
        public bool IsDeleted { get; set; }
    }
}