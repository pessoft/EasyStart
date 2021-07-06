using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class RecommendedProductRepository: Repository<RecommendedProductModel, int>
    {
        public RecommendedProductRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}