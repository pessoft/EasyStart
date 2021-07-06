using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class PromotionCouponRepository : Repository<CouponModel, int>
    {
        public PromotionCouponRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}