using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services
{
    public class OrderableLogic : IOrderableLogic
    {
        public void UpdateOrder<T>(IBaseRepository<T, int> repository, List<UpdaterOrderNumber> items)
            where T : class, IEntityOrderable<int>
        {
            if (items == null || !items.Any())
                return;

            var dict = items.ToDictionary(p => p.Id, p => p.OrderNumber);
            var ids = dict.Keys.ToList();
            var data = repository.Get(p => ids.Contains(p.Id)).ToList();

            data.ForEach(p => p.OrderNumber = dict[p.Id]);
        }
    }
}