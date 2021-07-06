using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class PromotionStockRepository : Repository<StockModel, int>
    {
        public PromotionStockRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}