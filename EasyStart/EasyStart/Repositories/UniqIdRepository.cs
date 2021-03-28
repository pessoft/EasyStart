using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class UniqIdRepository<T> : BaseRepository<T>, IUniqIdEntityRepository<T>
        where T : UniqIdEntity
    {
        public UniqIdRepository(DbContext dbContext): base(dbContext)
        {}
       
        public virtual T Get(string uniqId)
        {
            return db.FirstOrDefault(p => p.UniqId == uniqId);
        }

        public override void Update(T item)
        {
            var savedItem = Get(item.UniqId);
            Update(savedItem, item);
        }

        public override void Update(List<T> items)
        {
            if (items == null || !items.Any())
                return;

            var dict = items.ToDictionary(p => p.UniqId);
            var ids = dict.Keys.ToList();
            var savedItems = Get(p => ids.Contains(p.UniqId)).ToList();

            savedItems.ForEach(savedItem =>
            {
                var item = dict[savedItem.UniqId];

                UpdateWithotSaveChages(savedItem, item);
            });

            context.SaveChanges();
        }
    }
}