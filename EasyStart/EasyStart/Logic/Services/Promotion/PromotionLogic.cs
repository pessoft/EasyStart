using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Models;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyStart.Logic.Services.Promotion
{
    public class PromotionLogic : IPromotionLogic
    {
        private readonly IRepository<PromotionNewsModel, int> newsRepository;
        private readonly IRepository<StockModel, int> stockRepository;
        private readonly IRepository<CouponModel, int> couponRepository;
        private readonly IRepository<PromotionCashbackSetting, int> promotionCashbackSettingRepository;
        private readonly IRepository<PromotionPartnerSetting, int> promotionPartnerSettingRepository;
        private readonly IRepository<PromotionSectionSetting, int> promotionSectionSettingRepository;
        private readonly IRepository<PromotionSetting, int> promotionSettingRepository;
        private readonly IContainImageLogic imageLogic;

        public PromotionLogic(
            IRepositoryFactory repositoryFactory,
            IContainImageLogic imageLogic)
        {
            newsRepository = repositoryFactory.GetRepository<PromotionNewsModel, int>();
            stockRepository = repositoryFactory.GetRepository<StockModel, int>();
            couponRepository = repositoryFactory.GetRepository<CouponModel, int>();
            promotionCashbackSettingRepository = repositoryFactory.GetRepository<PromotionCashbackSetting, int>();
            promotionPartnerSettingRepository = repositoryFactory.GetRepository<PromotionPartnerSetting, int>();
            this.imageLogic = imageLogic;
        }

        public PromotionCashbackSetting GetPromotionCashbackSetting(int branchId)
        {
            return promotionCashbackSettingRepository.Get(p => p.BranchId == branchId).FirstOrDefault();
        }

        public PromotionPartnerSetting GetPromotionPartnerSetting(int branchId)
        {
            return promotionPartnerSettingRepository.Get(p => p.BranchId == branchId).FirstOrDefault();
        }

        public IEnumerable<CouponModel> GetCoupons(int branchId)
        {
            return couponRepository.Get(p => !p.IsDeleted && p.BranchId == branchId);
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

        public CouponModel SaveCoupon(CouponModel coupon)
        {
            coupon.IsDeleted = false;

            var oldCoupon = couponRepository.Get(coupon.Id);

            if (oldCoupon != null)
            {
                coupon.CountUsed = oldCoupon.CountUsed;
                coupon.UniqId = oldCoupon.UniqId;

                RemoveCoupon(oldCoupon.Id);
            }
            else
                coupon.UniqId = Guid.NewGuid();


            return couponRepository.Create(coupon);
        }

        public void RemoveCoupon(int id)
        {
            var forRemove = couponRepository.Get(id);

            couponRepository.Remove(forRemove);
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

        public PromotionCashbackSetting SavePromotionCashbackSetting(PromotionCashbackSetting setting)
        {
            var isNewSetting = promotionCashbackSettingRepository.Get(setting.Id) == null;

            if (isNewSetting)
                return promotionCashbackSettingRepository.Create(setting);

            return promotionCashbackSettingRepository.Update(setting);
        }

        public PromotionPartnerSetting SavePromotionPartnerSetting(PromotionPartnerSetting setting)
        {
            var isNewSetting = promotionPartnerSettingRepository.Get(setting.Id) == null;

            if (isNewSetting)
                return promotionPartnerSettingRepository.Create(setting);

            return promotionPartnerSettingRepository.Update(setting);
        }

        public PromotionGeneralSetting GetPromotionGeneralSetting(int branchId)
        {
            return new PromotionGeneralSetting
            {
                Sections = GetPromotionSectionSettings(branchId).ToList(),
                Setting = GetPromotionSetting(branchId)

            };
        }

        public IEnumerable<PromotionSectionSetting> GetPromotionSectionSettings(int branchId)
        {
            return promotionSectionSettingRepository.Get(p => p.BranchId == branchId)
                .OrderBy(p => p.Priorety);
        }

        public PromotionSetting GetPromotionSetting(int branchId)
        {
            return promotionSettingRepository.Get(p => p.BranchId == branchId).FirstOrDefault();
        }

        public PromotionGeneralSetting SavePromotionGeneralSettings(PromotionGeneralSetting setting)
        {
            if (setting == null || !setting.Sections.Any() || setting.Setting == null)
                throw new EmptyPromotionGeneralSettingException();

            return new PromotionGeneralSetting
            {
                Sections = SavePromotionSectionSettings(setting.Sections).ToList(),
                Setting = SavePromotionSetting(setting.Setting)
            };
        }

        public IEnumerable<PromotionSectionSetting> SavePromotionSectionSettings(IEnumerable<PromotionSectionSetting> sectionSettings)
        {
            return promotionSectionSettingRepository.CreateOrUpdate(sectionSettings);
        }

        public PromotionSetting SavePromotionSetting(PromotionSetting setting)
        {
            return promotionSettingRepository.CreateOrUpdate(setting);
        }
    }
}