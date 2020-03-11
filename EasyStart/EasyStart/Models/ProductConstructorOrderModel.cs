using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductConstructorOrderModel
    {
        public int CategoryId { get; set; }
        public int Count { get; set; }
        /// <summary>
        /// key - Ingredient Id
        /// value - Ingredient count
        /// </summary>
        public Dictionary<int, int> IngredientCount { get; set; }
    }
}