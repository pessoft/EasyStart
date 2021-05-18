using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Branch
{
    public class UtilsLogic : IUtilsLogic
    {
        public string SaveImage(HttpServerUtilityBase server, HttpRequestBase request)
        {
            string urlSavedImage = null;
            var imagePath = "Images/Products";

            foreach (string file in request.Files)
            {
                var upload = request.Files[file];
                if (upload != null)
                {
                    string fileName = System.IO.Path.GetFileName(upload.FileName);
                    string ext = fileName.Substring(fileName.LastIndexOf("."));
                    string newFileName = String.Format(@"{0}{1}", System.Guid.NewGuid(), ext);

                    upload.SaveAs(server.MapPath($"~/{imagePath}/${newFileName}"));
                    urlSavedImage = $"../${imagePath}/{newFileName}";
                }
                else
                    throw new Exception("Не удалось загрузить изображение");
            }

            return urlSavedImage;
        }
    }
}