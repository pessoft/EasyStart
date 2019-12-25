using EasyStart.Models;
using EasyStart.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class PromotionDefaultSetting
    {
        private int branchId;
        private string zoneId;

        public PromotionDefaultSetting(int branchId)
        {
            this.branchId = branchId;

            SetZoneId();
        }

        public void SaveSettings()
        {
            SavePromotionCashbackSetting();
            SavePromotionPartnerSetting();
            SavePromotionSectionSettings();
        }

        private void SetZoneId()
        {
            var deliverSetting = DataWrapper.GetDeliverySetting(branchId);
            zoneId = deliverSetting.ZoneId;
        }

        private void SavePromotionCashbackSetting()
        {
            var setting = GetPromotionCashbackSetting();
            DataWrapper.SavePromotionCashbackSetting(setting);
        }

        private void SavePromotionPartnerSetting()
        {
            var setting = GetPromotionPartnerSetting();
            DataWrapper.SavePromotionPartnerSetting(setting);
        }

        private void SavePromotionSectionSettings()
        {
            var settings = GetPromotionSectionSettings();
            DataWrapper.SavePromotionSectionSettings(settings);
        }

        private PromotionCashbackSetting GetPromotionCashbackSetting()
        {
            var setting = new PromotionCashbackSetting()
            {
                BranchId = branchId,
                IsUseCashback = false,
                ReturnedValue = 10,
                PaymentValue = 40,
                DateSave = DateTime.Now.GetDateTimeNow(zoneId)
            };

            return setting;
        }

        private PromotionPartnerSetting GetPromotionPartnerSetting()
        {
            var setting = new PromotionPartnerSetting()
            {
                BranchId = branchId,
                IsUsePartners = false,
                BonusValue = 20,
                CashBackReferralValue = 20,
                IsCashBackReferralOnce = true,
                TypeBonusValue = DiscountType.Percent,
                DateSave = DateTime.Now.GetDateTimeNow(zoneId)
            };

            return setting;
        }

        private List<PromotionSectionSetting> GetPromotionSectionSettings()
        {
            var settings = new List<PromotionSectionSetting>();
            settings.Add(GetPromotionSectionSetting(PromotionSection.Partners, 0));
            settings.Add(GetPromotionSectionSetting(PromotionSection.Coupon, 1));
            settings.Add(GetPromotionSectionSetting(PromotionSection.Stock, 2));

            return settings;
        }

        private PromotionSectionSetting GetPromotionSectionSetting(PromotionSection promotionSection, int priority)
        {
            var setting = new PromotionSectionSetting()
            {
                BranchId = branchId,
                Priorety = priority,
                PromotionSection = promotionSection
            };

            return setting;
        }
    }
}