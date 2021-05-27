using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace EasyStart.Logic.Services
{
    public class ContainImageLogic : IContainImageLogic
    {
        private const string defaultImage = "../Images/default-image.jpg";

        public void PrepareImage(IContainImage item)
        {
            if (!System.IO.File.Exists(HostingEnvironment.MapPath(GetVirtualPath(item.Image))))
                item.Image = defaultImage;
        }

        public void RemoveOldImage(IContainImage olditem, IContainImage newItem)
        {
            var oldImageVirtualPage = GetVirtualPath(olditem.Image);
            if (olditem.Image != newItem.Image
                    && olditem.Image != defaultImage
                   && System.IO.File.Exists(HostingEnvironment.MapPath(oldImageVirtualPage)))
            {
                System.IO.File.Delete(HostingEnvironment.MapPath(oldImageVirtualPage));
            }
        }

        private string GetVirtualPath(string path)
        {
            return path.StartsWith("../") ? path.Replace("../", "~/") : path;
        }
    }
}