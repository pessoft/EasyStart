using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services
{
    public interface IOrderableLogic
    {
        void UpdateOrder<T>(IBaseRepository<T, int> repository, List<UpdaterOrderNumber> items)
            where T : class, IEntityOrderable<int>;
    }
}
