using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic
{
    public class PreprocessorDataAPI
    {
        public static  void ChangeImagePath(IContainImage item)
        {
            if (!string.IsNullOrEmpty(item.Image))
            {
                item.Image = item.Image.Substring(2);
            }
        }
    }
}