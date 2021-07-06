using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class OrderRepository : Repository<OrderModel, int>
    {
        public OrderRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}