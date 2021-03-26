using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
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

        public virtual T Get(int id)
        {
            return db.FirstOrDefault(p => p.Id == id);
        }

        public virtual void Remove(T item)
        {
            db.Remove(item);
            context.SaveChanges();
        }

        public virtual void Update(T item)
        {
            var savedItem = Get(item.Id);
            context.Entry(savedItem).State = EntityState.Detached;

            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }

        public virtual void Update(List<T> items)
        {
            if (items == null || !items.Any())
                return;

            var dict = items.ToDictionary(p => p.Id);
            var ids = dict.Keys.ToList();
            var savedItems = Get(p => ids.Contains(p.Id)).ToList();

            savedItems.ForEach(savedItem => 
            {
                var item = dict[savedItem.Id];

                context.Entry(savedItem).State = EntityState.Detached;
                context.Entry(item).State = EntityState.Modified;
            });
            
            context.SaveChanges();
        }
    }
}