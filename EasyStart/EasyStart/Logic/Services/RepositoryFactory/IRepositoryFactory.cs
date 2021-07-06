using EasyStart.Models;
using EasyStart.Repository;
using System;

namespace EasyStart.Logic.Services.RepositoryFactory
{
    public interface IRepositoryFactory
    {
        IRepository<T, U> GetRepository<T,U>()
            where T: class, IBaseEntity<U>, new()
            where U: IEquatable<U>;
    }
}
