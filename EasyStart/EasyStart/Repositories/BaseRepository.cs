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
            var addedItem = CreateWithouSaveChanges(item);
            context.SaveChanges();

            return addedItem;
        }

        protected T CreateWithouSaveChanges(T item)
        {
            var addedItem = db.Add(item);

            return addedItem;
        }

        public virtual List<T> Create(List<T> items)
        {
            if (items == null || !items.Any())
                return items;

            var newItems = new List<T>();
            items.ForEach(item =>
            {
                var addedItem = CreateWithouSaveChanges(item);
                newItems.Add(addedItem);
            });
            
            context.SaveChanges();

            return newItems;
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
            RemoveWithotSaveChages(item);
            context.SaveChanges();
        }
        public virtual void Remove(List<T> items)
        {
            if (items == null || !items.Any())
                return;

            items.ForEach(p => RemoveWithotSaveChages(p));
            context.SaveChanges();
        }

        protected virtual void RemoveWithotSaveChages(T item)
        {
            db.Remove(item);
        }

        public abstract T Update(T item);

        public abstract List<T> Update(List<T> items);

        protected T Update(T savedItem, T item)
        {
            UpdateWithotSaveChages(savedItem, item);
            context.SaveChanges();

            return item;
        }

        protected void UpdateWithotSaveChages(T savedItem, T item)
        {
            context.Entry(savedItem).State = EntityState.Detached;
            context.Entry(item).State = EntityState.Modified;
        }
    }
}