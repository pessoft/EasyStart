using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class DefaultRepository<T> : BaseRepository<T>, IDefaultEntityRepository<T>
        where T : BaseEntity
    {
        public DefaultRepository(DbContext dbContext): base(dbContext)
        {}
       
        public virtual T Get(int id)
        {
            return db.FirstOrDefault(p => p.Id == id);
        }

        public override void Update(T item)
        {
            var savedItem = Get(item.Id);
            Update(savedItem, item);
        }

        public override void Update(List<T> items)
        {
            if (items == null || !items.Any())
                return;

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
    }
}