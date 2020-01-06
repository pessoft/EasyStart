using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class OrderDetailProduct
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<ProductModel> Products { get; set; }
    }
}