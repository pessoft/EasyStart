using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class ConstructorCategory: IEntityOrderable<int>
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int BranchId { get; set; }
        public string Name { get; set; }
        public int MinCountIngredient { get; set; }
        public int MaxCountIngredient { get; set; }
        public StyleTypeIngredient StyleTypeIngredient { get; set; }
        public int OrderNumber { get; set; }
        public bool IsDeleted { get; set; }
    }
}