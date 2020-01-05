using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductConstructorIngredientModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int MaxCountIngredient { get; set; }
        public StyleTypeIngredient StyleTypeIngredient { get; set; }
        public int OrderNumber { get; set; }
        public List<IngredientModel> Ingredients { get; set; }
    }
}