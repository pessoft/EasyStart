using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class PromotionSetting
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public bool IsShowStockBanner { get; set; } = true;
        public bool IsShowNewsBanner { get; set; } = true;
    }
}