using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Promotion
{
    public class PromotionLogic : ContainImageLogic, IPromotionLogic
    {
        private IBaseRepository<PromotionNewsModel, int> newsRepository;
        private IBaseRepository<StockModel, int> stockRepository;
        
        public PromotionLogic(
            IBaseRepository<PromotionNewsModel, int> newsRepository,
            IBaseRepository<StockModel, int> stockRepository)
        {
            this.newsRepository = newsRepository;
            this.stockRepository = stockRepository;
        }

        public List<PromotionNewsModel> GetNews(int branchId)
        {
            return newsRepository.Get(p => p.BranchId == branchId).ToList();
        }

        public void RemoveNews(int newsId)
        {
            var news = newsRepository.Get(newsId);
            news.IsDeleted = true;

            newsRepository.Update(news);
        }

        public PromotionNewsModel SaveNews(PromotionNewsModel promotionNews)
        {
            PrepareImage(promotionNews);

            var oldNews = newsRepository.Get(promotionNews.Id);
            PromotionNewsModel savedNews;
            if (oldNews != null)
                savedNews = newsRepository.Update(promotionNews);
            else
                savedNews = newsRepository.Create(promotionNews);

            return savedNews;
        }

        public StockModel SaveStock(StockModel stock)
        {
            PrepareImage(stock);

            var oldStock = stockRepository.Get(stock.Id);

            if (oldStock != null)
            {
                oldStock.IsDeleted = true;
                stock.UniqId = oldStock.UniqId;

                stockRepository.Update(oldStock);
            }
            else
                stock.UniqId = Guid.NewGuid();

            return stockRepository.Create(stock);
        }
    }
}