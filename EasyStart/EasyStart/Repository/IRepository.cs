using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Repository
{
    public interface IRepository<T, U>
        where T: class
        where U: IEquatable<U>
    {
        T Create(T item);
        List<T> Create(List<T> items);
        IEnumerable<T> Get();
        T Get(U id);
        IEnumerable<T> Get(Func<T, bool> predicate);
        T Update(T item);
        List<T> Update(List<T> items);
        void Remove(T item);
        void Remove(List<T> items);
    }
}