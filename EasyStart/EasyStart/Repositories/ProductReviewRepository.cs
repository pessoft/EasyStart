using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class ProductReviewRepository : BaseRepository<ProductReview, int>
    {
        public ProductReviewRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}