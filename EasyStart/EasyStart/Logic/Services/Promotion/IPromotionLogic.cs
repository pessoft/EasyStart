using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
