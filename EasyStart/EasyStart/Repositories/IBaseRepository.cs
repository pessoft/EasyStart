using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public interface IBaseRepository<T> where T: class
    {
        T Create(T item);
        List<T> Create(List<T> items);
        IEnumerable<T> Get();
        IEnumerable<T> Get(Func<T, bool> predicate);
        T Update(T item);
        List<T> Update(List<T> items);
        void Remove(T item);
        void Remove(List<T> items);
    }
}