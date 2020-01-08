using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class CategoryModel : IContainImage
    { 
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int OrderNumber { get; set; }
        public bool Visible { get; set; } = true;
        public CategoryType CategoryType { get; set; } = CategoryType.Default;
        public bool IsDeleted { get; set; }
    }
}