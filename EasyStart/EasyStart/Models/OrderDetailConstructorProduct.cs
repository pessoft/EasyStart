using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class OrderDetailConstructorProduct
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryImage { get; set; }
        public List<IngredientModel> Ingredients { get; set; }
    }
}