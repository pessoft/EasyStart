using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class ProductAdditionalFillingRepository: Repository<ProductAdditionalFillingModal, int>
    {
        public ProductAdditionalFillingRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}