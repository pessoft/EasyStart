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
        private IBaseRepository<PromotionNewsModel, int> promotionNewsRepository;
        public PromotionLogic(IBaseRepository<PromotionNewsModel, int> promotionNewsRepository)
        {
            this.promotionNewsRepository = promotionNewsRepository;
        }

        public void RemovePromotionNews(int newsId)
        {
            var news = promotionNewsRepository.Get(newsId);
            news.IsDeleted = true;

            promotionNewsRepository.Update(news);
        }

        public PromotionNewsModel SaveNews(PromotionNewsModel promotionNews)
        {
            PrepareImage(promotionNews);

            var oldNews = promotionNewsRepository.Get(promotionNews.Id);
            PromotionNewsModel savedNews;
            if (oldNews != null)
                savedNews = promotionNewsRepository.Update(promotionNews);
            else
                savedNews = promotionNewsRepository.Create(promotionNews);

            return savedNews;
        }
    }
}