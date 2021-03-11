using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class RecommendedProductModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BrunchId { get; set; }
        public int ProductId { get; set; }
    }
}