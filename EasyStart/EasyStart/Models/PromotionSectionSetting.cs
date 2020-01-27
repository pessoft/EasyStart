using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class PromotionSectionSetting
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int Priorety { get; set; }
        public PromotionSection PromotionSection { get; set; }
        public PromotionSection Intersections { get; set; }
    }
}