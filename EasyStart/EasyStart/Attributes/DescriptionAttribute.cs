using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Attributes
{
    public class DescriptionAttribute : Attribute
    {
        public string Text;
        public DescriptionAttribute(string text) => Text = text;
    }
}