using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ProductHistoryModel: IContainImage
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string AdditionInfo { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public CategoryType CategoryType { get; set; }
        public bool IsDeleted { get; set; }
    }
}