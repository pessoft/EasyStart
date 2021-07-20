using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace EasyStart.Logic.Services
{
    public interface IContainImageLogic
    {
        void PrepareImage(IContainImage item);
        void PrepareImage(IEnumerable<IContainImage> items);
        void RemoveOldImage(IContainImage olditem, IContainImage newItem);
    }
}