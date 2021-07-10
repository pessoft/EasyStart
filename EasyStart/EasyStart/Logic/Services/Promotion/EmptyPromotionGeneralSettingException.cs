using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Promotion
{
    public class EmptyPromotionGeneralSettingException : Exception
    {
        public EmptyPromotionGeneralSettingException() : base("Пустая настройка")
        { }
    }
}