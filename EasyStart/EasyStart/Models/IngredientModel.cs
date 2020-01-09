using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class IngredientModel: IContainImage
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Name { get; set; }
        public string AdditionaInfo { get; set; }
        public double Price { get; set; }
        public int MinRequiredCount { get; set; }
        public int MaxAddCount { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
    }
}