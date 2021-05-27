using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class BaseRepository<T, U> : IBaseRepository<T, U>
        where T : class, IBaseEntity<U>
        where U: IEquatable<U>
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

        public virtual T Get(U id)
        {
            return db.FirstOrDefault(p => p.Id.Equals(id));
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

        public virtual T Update(T item)
        {
            var savedItem = Get(item.Id);
            return Update(savedItem, item);
        }

        public virtual List<T> Update(List<T> items)
        {
            if (items != null && items.Any())
            {
                var dict = items.ToDictionary(p => p.Id);
                var ids = dict.Keys.ToList();
                var savedItems = Get(p => ids.Contains(p.Id)).ToList();

                savedItems.ForEach(savedItem =>
                {
                    var item = dict[savedItem.Id];

                    UpdateWithotSaveChages(savedItem, item);
                });

                context.SaveChanges();
            }

            return items;
        }

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