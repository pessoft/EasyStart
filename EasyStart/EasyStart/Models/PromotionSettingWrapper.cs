using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class PromotionSettingWrapper
    {
        public List<PromotionSectionSetting> Sections { get; set; }

        public PromotionSetting Setting { get; set; }
    }
}