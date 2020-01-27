﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class OrderInfoParams
    {
        public SettingModel Setting { get; set; }
        public List<ProductModel> Products{get;set;}
        public List<ProductModel> BonusProducts { get; set; }
        public List<CategoryModel> CategoryConstructor { get; set; }
        public List<IngredientModel> ConstructorIngredients { get; set; }
    }
}