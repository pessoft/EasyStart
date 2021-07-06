using EasyStart.Models;
using EasyStart.Repository;
using System;
using System.Data.Entity;

namespace EasyStart.Logic.Services.RepositoryFactory
{
    public class RepositoryFactory : IRepositoryFactory
    {
        protected DbContext context;
        
        public RepositoryFactory(DbContext dbContext)
        {
            context = dbContext;
        }

        public IRepository<T, U> GetRepository<T, U>()
            where T : class, IBaseEntity<U>, new()
            where U : IEquatable<U>
        {
            return new Repository<T, U>(context);
        }
    }
}
