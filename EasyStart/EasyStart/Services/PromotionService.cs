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
            var branch = branchLogic.Get();
            promotionNews.BranchId = branch.Id;

            return promotionLogic.SaveNews(promotionNews);
        }

        public void RemovePromotionNews(int newsId)
        {
            promotionLogic.RemoveNews(newsId);
        }

        public List<PromotionNewsModel> GetNews()
        {
            var branch = branchLogic.Get();

            return promotionLogic.GetNews(branch.Id);
        }

        public StockModel SaveStock(StockModel stock)
        {
            var branch = branchLogic.Get();
            stock.BranchId = branch.Id;

            return promotionLogic.SaveStock(stock);
        }

        public void RemoveStock(int stockId)
        {
            promotionLogic.RemoveStock(stockId);
        }
    }
}