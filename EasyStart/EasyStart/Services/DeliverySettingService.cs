using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class DeliverySettingService
    {
        private readonly IDeliverySettingLogic deliverySettingLogic;

        public DeliverySettingService(IDeliverySettingLogic deliverySettingLogic)
        {
            this.deliverySettingLogic = deliverySettingLogic;
        }

        public DeliverySettingModel SaveDeliverySetting(DeliverySettingModel setting)
        {
            return deliverySettingLogic.SaveDeliverySetting(setting);
        }

        public DeliverySettingModel GetByBranchId(int branchId)
        {
            return deliverySettingLogic.GetByBranchId(branchId);
        }

        public List<AreaDeliveryModel> GetAreaDeliveris(int delvierySettingId)
        {
            return deliverySettingLogic.GetAreaDeliveris(delvierySettingId);
        }

        public List<AreaDeliveryModel> SaveAreaDeliveries(List<AreaDeliveryModel> areaDeliveries)
        {
            return deliverySettingLogic.SaveAreaDeliveries(areaDeliveries);
        }
    }
}