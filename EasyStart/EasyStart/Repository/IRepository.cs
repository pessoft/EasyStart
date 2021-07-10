using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Repository
{
    public interface IRepository<T, U>
        where T: class, IBaseEntity<U>
        where U: IEquatable<U>
    {
        T Create(T item);
        IEnumerable<T> Create(IEnumerable<T> items);
        IEnumerable<T> Get();
        T Get(U id);
        IEnumerable<T> Get(Func<T, bool> predicate);
        T Update(T item);
        IEnumerable<T> Update(IEnumerable<T> items);
        void Remove(T item);
        void Remove(IEnumerable<T> items);
        T CreateOrUpdate(T item);
        IEnumerable<T> CreateOrUpdate(IEnumerable<T> items);
    }
}