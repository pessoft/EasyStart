using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class StockModel
    {
        public int Id { get; set; }
        public StockType StockType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Discount { get; set; }
        public string Image { get; set; }
        public bool Visible { get; set; }

    }
}