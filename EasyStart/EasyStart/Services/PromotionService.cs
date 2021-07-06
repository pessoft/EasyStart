using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.Promotion;
using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class PromotionService
    {
        private IPromotionLogic promotionLogic;
        private IBranchLogic branchLogic;
        public PromotionService(
            IPromotionLogic promotionLogic,
            IBranchLogic branchLogic)
        {
            this.promotionLogic = promotionLogic;
            this.branchLogic = branchLogic;
        }

        public PromotionNewsModel SaveNews(PromotionNewsModel promotionNews)
        {
            promotionNews.BranchId = GetBranchId();

            return promotionLogic.SaveNews(promotionNews);
        }

        public void RemovePromotionNews(int newsId)
        {
            promotionLogic.RemoveNews(newsId);
        }

        public List<PromotionNewsModel> GetNews()
        {
            return promotionLogic.GetNews(GetBranchId());
        }

        public StockModel SaveStock(StockModel stock)
        {
            stock.BranchId = GetBranchId();

            return promotionLogic.SaveStock(stock);
        }

        public void RemoveStock(int stockId)
        {
            promotionLogic.RemoveStock(stockId);
        }

        public List<StockModel> GetStocks()
        {
            return promotionLogic.GetStocks(GetBranchId());
        }

        public IEnumerable<CouponModel> GetCoupons()
        {
            return promotionLogic.GetCoupons(GetBranchId());
        }

        public CouponModel SaveCoupon(CouponModel coupon)
        {
            coupon.BranchId = GetBranchId();

            return promotionLogic.SaveCoupon(coupon);
        }

        public void RemoveCoupon(int id)
        {
            promotionLogic.RemoveCoupon(id);
        }

        public PromotionCashbackSetting GetPromotionCashbackSetting()
        {
            return promotionLogic.GetPromotionCashbackSetting(GetBranchId());
        }

        public PromotionPartnerSetting GetPromotionPartnerSetting()
        {
            return promotionLogic.GetPromotionPartnerSetting(GetBranchId());
        }

        private int GetBranchId()
        {
            return branchLogic.Get().Id;
        }
    }
}