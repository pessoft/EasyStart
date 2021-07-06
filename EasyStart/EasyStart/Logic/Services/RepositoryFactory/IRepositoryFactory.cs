using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.RepositoryFactory
{
    public interface IRepositoryFactory
    {
        IRepository<T, U> GetRepository<T,U>()
            where T: class, IBaseEntity<U>, new()
            where U: IEquatable<U>;
    }
}
