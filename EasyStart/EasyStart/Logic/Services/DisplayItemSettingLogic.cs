using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services
{
    public class DisplayItemSettingLogic : IDisplayItemSettingLogic
    {
        public void UpdateOrder<T>(IRepository<T, int> repository, List<UpdaterOrderNumber> items)
            where T : class, IEntityOrderable<int>
        {
            if (items == null || !items.Any())
                return;

            var dict = items.ToDictionary(p => p.Id, p => p.OrderNumber);
            var ids = dict.Keys.ToList();
            var data = repository.Get(p => ids.Contains(p.Id)).ToList();

            data.ForEach(p => p.OrderNumber = dict[p.Id]);

            repository.Update(data);
        }

        public void UpdateVisible<T>(IRepository<T, int> repository, UpdaterVisible update)
            where T : class, IEntityVisible<int>
        {
            if (update == null || update.Id < 1)
                return;

            var entity = repository.Get(update.Id);

            if (entity != null)
            {
                entity.Visible = update.Visible;

                repository.Update(entity);
            }
        }
    }
}