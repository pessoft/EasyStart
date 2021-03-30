using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class AreaDeliveryRepository : UniqIdRepository<AreaDeliveryModel>
    {
        public AreaDeliveryRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}