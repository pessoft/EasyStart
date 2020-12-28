using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class BranchRepository : IRepository<BranchModel>, IDisposable
    {
        protected bool disposed = false;
        private DbContext context;
        private DbSet<BranchModel> db;

        public BranchRepository(DbContext dbContext)
        {
            context = dbContext;
            db = context.Set<BranchModel>();
        }
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            context.Dispose();
            GC.SuppressFinalize(this);
        }

        public BranchModel Create(BranchModel item)
        {
            var result = db.Add(item);
            context.SaveChanges();

            return result;
        }

        public IEnumerable<BranchModel> Get()
        {
            return db.AsEnumerable();
        }

        public IEnumerable<BranchModel> Get(Func<BranchModel, bool> predicate)
        {
            return db.Where(predicate).AsEnumerable();
        }

        public BranchModel Get(int id)
        {
            return db.FirstOrDefault(p => p.Id == id);
        }

        public void Remove(BranchModel item)
        {
            db.Remove(item);
            context.SaveChanges();
        }

        public void Update(BranchModel item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}