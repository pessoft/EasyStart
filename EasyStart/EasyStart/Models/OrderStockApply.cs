using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class OrderStockApply
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int StockId { get; set; }
    }
}