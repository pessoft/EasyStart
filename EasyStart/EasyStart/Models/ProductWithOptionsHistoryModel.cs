using EasyStart.Migrations;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductWithOptionsHistoryModel : ProductHistoryModel
    {
        public double PriceWithOptions { get; set; }
        public Dictionary<int, AdditionalOption> AdditionalOptions { get; set; }
        public Dictionary<int, ProductOption.AdditionalFilling> AdditionalFillings { get; set; }
    }
}