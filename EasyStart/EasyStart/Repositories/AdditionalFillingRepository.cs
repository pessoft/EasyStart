using EasyStart.Models;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class AdditionalFillingRepository : BaseRepository<AdditionalFilling, int>
    {
        public AdditionalFillingRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}