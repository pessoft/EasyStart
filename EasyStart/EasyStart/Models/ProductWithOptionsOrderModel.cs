using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductWithOptionsOrderModel
    {
        public int CategoryId { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }
        public Dictionary<int, int> AdditionalOptions { get; set; }
        public List<int> AdditionalFillings { get; set; }
    }
}