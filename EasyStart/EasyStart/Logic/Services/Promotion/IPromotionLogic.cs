using EasyStart.Models;
using System.Collections.Generic;

namespace EasyStart.Logic.Services.Promotion
{
    public interface IPromotionLogic
    {
        PromotionNewsModel SaveNews(PromotionNewsModel promotionNews);
        List<PromotionNewsModel> GetNews(int branchId);
        void RemoveNews(int newsId);
        StockModel SaveStock(StockModel stock);
        List<StockModel> GetStocks(int branchId);
        void RemoveStock(int stockId);
        IEnumerable<CouponModel> GetCoupons(int branchId);
        CouponModel SaveCoupon(CouponModel coupon);
        void RemoveCoupon(int id);
        PromotionCashbackSetting GetPromotionCashbackSetting(int branchId);
        PromotionPartnerSetting GetPromotionPartnerSetting(int branchId);
        PromotionCashbackSetting SavePromotionCashbackSetting(PromotionCashbackSetting setting);
        PromotionPartnerSetting SavePromotionPartnerSetting(PromotionPartnerSetting setting);
        PromotionGeneralSetting GetPromotionGeneralSetting(int branchId);
        IEnumerable<PromotionSectionSetting> GetPromotionSectionSettings(int branchId);
        PromotionSetting GetPromotionSetting(int branchId);

        PromotionGeneralSetting SavePromotionGeneralSettings(PromotionGeneralSetting setting);
        IEnumerable<PromotionSectionSetting> SavePromotionSectionSettings(IEnumerable<PromotionSectionSetting> sectionSettings);
        PromotionSetting SavePromotionSetting(PromotionSetting setting);
    }
}
