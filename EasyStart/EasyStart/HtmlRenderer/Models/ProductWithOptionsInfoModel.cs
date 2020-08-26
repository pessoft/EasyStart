using EasyStart.Models.ProductOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.HtmlRenderer.Models
{
    public class ProductWithOptionsInfoModel: ProductInfoModel
    {
        public List<AdditionalOption> AdditionalOptions { get; set; }
        public List<AdditionalFilling> AdditionalFillings { get; set; }
    }
}