using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class ConstructorCategoryRepository : Repository<ConstructorCategory, int>
    {
        public ConstructorCategoryRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}