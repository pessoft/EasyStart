using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.HtmlRenderer.Models
{
    public class ConstructorProductInfoModel
    {
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public int ProductCount { get; set; }
        public List<IngredientOrderModel> Ingredients { get; set; }
    }
}