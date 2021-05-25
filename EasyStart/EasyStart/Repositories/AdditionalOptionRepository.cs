using EasyStart.Models;
using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class AdditionalOptionRepository : BaseRepository<AdditionalOption, int>
    {
        public AdditionalOptionRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}