using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class InegrationSystemRepository : BaseRepository<IntegrationSystemModel, int>
    {
        public InegrationSystemRepository(DbContext dbContext):base(dbContext) 
        { }
    }
}