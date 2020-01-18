using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.HtmlRenderer.Models
{
    public class IngredientOrderModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}