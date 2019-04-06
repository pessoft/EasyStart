using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string NameProduct { get; set; }
        public string Discription { get; set; }
        public double Price { get; set; }
        public string ProductImage { get; set; }
    }
}