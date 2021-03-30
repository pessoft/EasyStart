using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public interface IDefaultEntityRepository<T> : IBaseRepository<T> where T: BaseEntity
    {
        T Get(int id);
    }
}