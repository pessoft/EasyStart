using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
        where T : class
    {
        protected bool disposed = false;
        protected DbContext context;
        protected DbSet<T> db;

        public BaseRepository(DbContext dbContext)
        {
            context = dbContext;
            db = context.Set<T>();
        }
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            context.Dispose();
            GC.SuppressFinalize(this);
        }
        public virtual T Create(T item)
        {
            var addedItem = db.Add(item);
            context.SaveChanges();

            return addedItem;
        }

        public virtual IEnumerable<T> Get()
        {
            return db.AsEnumerable();
        }

        public virtual IEnumerable<T> Get(Func<T, bool> predicate)
        {
            return db.Where(predicate).AsEnumerable();
        }

        public virtual void Remove(T item)
        {
            db.Remove(item);
            context.SaveChanges();
        }
        public abstract void Update(T item);
        public abstract void Update(List<T> items);

        protected void Update(T savedItem, T item)
        {
            UpdateWithotSaveChages(savedItem, item);
            context.SaveChanges();
        }

        protected void UpdateWithotSaveChages(T savedItem, T item)
        {
            context.Entry(savedItem).State = EntityState.Detached;
            context.Entry(item).State = EntityState.Modified;
        }
    }
}