using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class ProductAdditionOptionItemRepository: BaseRepository<ProductAdditionalOptionModal, int>
    {
        public ProductAdditionOptionItemRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}