using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public interface IRepository<T> where T: class
    {
        T Create(T item);
        IEnumerable<T> Get();
        IEnumerable<T> Get(Func<T, bool> predicate);
        T Get(int id);

        void Update(T item);
        void Remove(T item);       
    }
}