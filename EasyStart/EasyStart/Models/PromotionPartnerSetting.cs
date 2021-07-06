using EasyStart.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Models
{
    public class PromotionPartnerSetting: IBaseEntity<int>
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public bool IsUsePartners { get; set; }
        public int CashBackReferralValue { get; set; }
        public bool IsCashBackReferralOnce { get; set; }
        public DiscountType TypeBonusValue { get; set; }
        public int BonusValue { get; set; }
        public DateTime DateSave { get; set; }
    }
}