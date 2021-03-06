﻿using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.IntegrationSystem
{
    public class IntegrationSystemFactory : IIntegrationSystemFactory
    {
        public IIntegrationSystem GetIntegrationSystem(IntegrationSystemModel integrationSystemSetting)
        {
            switch (integrationSystemSetting.Type)
            {
                case IntegrationSystemType.FrontPad:
                    return new FrontPad(integrationSystemSetting);
                case IntegrationSystemType.Iiko:
                    return new Iiko(integrationSystemSetting);
                default:
                    return new WithoutIntegration(integrationSystemSetting);
            }
        }
    }
}