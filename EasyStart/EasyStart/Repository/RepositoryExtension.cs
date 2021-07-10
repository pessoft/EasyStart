using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Repository
{
    public static class RepositoryExtension
    {
        public static void MarkAsDeleted<T, U>(this IRepository<T, U> repository, IEntityMarkAsDeleted<U> item)
            where T : class, IBaseEntity<U>
            where U : IEquatable<U>
        {
            item.IsDeleted = true;
            repository.Update((T)item);
        }

        public static void MarkAsDeleted<T, U>(this IRepository<T, U> repository, IEnumerable<IEntityMarkAsDeleted<U>> items)
            where T : class, IBaseEntity<U>
            where U : IEquatable<U>
        {
            foreach (var item in items)
                item.IsDeleted = true;

            repository.Update(items.Cast<T>());
        }
    }
}