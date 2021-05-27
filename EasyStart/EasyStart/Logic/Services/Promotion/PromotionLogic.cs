﻿using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Promotion
{
    public class PromotionLogic : IPromotionLogic
    {
        private readonly IBaseRepository<PromotionNewsModel, int> newsRepository;
        private readonly IBaseRepository<StockModel, int> stockRepository;
        private readonly IContainImageLogic imageLogic;

        public PromotionLogic(
            IBaseRepository<PromotionNewsModel, int> newsRepository,
            IBaseRepository<StockModel, int> stockRepository,
            IContainImageLogic imageLogic)
        {
            this.newsRepository = newsRepository;
            this.stockRepository = stockRepository;
            this.imageLogic = imageLogic;
        }

        public List<PromotionNewsModel> GetNews(int branchId)
        {
            return newsRepository.Get(p => p.BranchId == branchId && !p.IsDeleted).ToList();
        }

        public List<StockModel> GetStocks(int branchId)
        {
            return stockRepository.Get(p => p.BranchId == branchId && !p.IsDeleted).ToList();
        }

        public void RemoveNews(int newsId)
        {
            var news = newsRepository.Get(newsId);
            news.IsDeleted = true;

            newsRepository.Update(news);
        }

        public void RemoveStock(int stockId)
        {
            var stock = stockRepository.Get(stockId);
            stock.IsDeleted = true;

            stockRepository.Update(stock);
        }

        public PromotionNewsModel SaveNews(PromotionNewsModel promotionNews)
        {
            imageLogic.PrepareImage(promotionNews);

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
            imageLogic.PrepareImage(stock);

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