using EasyStart.Models;
using EasyStart.Models.FCMNotification;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EasyStart.Repositories
{
    public class FCMDeviceRepository : BaseRepository<FCMDeviceModel, int>
    {
        public FCMDeviceRepository(DbContext dbContext) : base(dbContext)
        { }
    }
}