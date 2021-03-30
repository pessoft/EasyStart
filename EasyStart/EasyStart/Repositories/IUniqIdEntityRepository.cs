using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public interface IUniqIdEntityRepository<T> : IBaseRepository<T> where T: UniqIdEntity
    {
        T Get(string uniqId);
    }
}