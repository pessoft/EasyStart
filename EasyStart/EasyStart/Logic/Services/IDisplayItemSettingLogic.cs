using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services
{
    public interface IDisplayItemSettingLogic
    {
        void UpdateOrder<T>(IBaseRepository<T, int> repository, List<UpdaterOrderNumber> items)
            where T : class, IEntityOrderable<int>;

        void UpdateVisible<T>(IBaseRepository<T, int> repository, UpdaterVisible update)
            where T : class, IEntityVisible<int>;
    }
}
