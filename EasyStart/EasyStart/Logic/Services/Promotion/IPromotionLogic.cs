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
        void RemovePromotionNews(int newsId);
        StockModel SaveStock(StockModel stock);
    }
}
