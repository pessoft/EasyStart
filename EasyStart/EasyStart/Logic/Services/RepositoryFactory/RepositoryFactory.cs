using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
