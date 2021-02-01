using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class OrderService
    {
        private readonly IRepository<OrderModel> repository;
        
        public OrderService(IRepository<OrderModel> repository)
        {
            this.repository = repository;
        }

        public OrderModel Get(int id)
        {
            var result = repository.Get(id);

            return result;
        }
    }
}