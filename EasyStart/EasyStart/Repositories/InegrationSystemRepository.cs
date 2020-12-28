using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class InegrationSystemRepository : IRepository<IntegrationSystemModal>, IDisposable
    {
        protected bool disposed = false;
        private DbContext context;
        private DbSet<IntegrationSystemModal> db;

        public InegrationSystemRepository(DbContext dbContext)
        {
            context = dbContext;
            db = context.Set<IntegrationSystemModal>();
        }
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            context.Dispose();
            GC.SuppressFinalize(this);
        }
        public IntegrationSystemModal Create(IntegrationSystemModal item)
        {
            var addedItem = db.Add(item);
            context.SaveChanges();

            return addedItem;
        }

        public IEnumerable<IntegrationSystemModal> Get()
        {
            return db.AsEnumerable();
        }

        public IEnumerable<IntegrationSystemModal> Get(Func<IntegrationSystemModal, bool> predicate)
        {
            return db.Where(p => predicate(p)).AsEnumerable();
        }

        public IntegrationSystemModal Get(int id)
        {
            return db.FirstOrDefault(p => p.Id == id);
        }

        public void Remove(IntegrationSystemModal item)
        {
            db.Remove(item);
            context.SaveChanges();
        }

        public void Update(IntegrationSystemModal item)
        {
            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}