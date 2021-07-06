using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.DeliverySetting;
using EasyStart.Models;
using EasyStart.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class DeliverySettingService
    {
        private readonly IDeliverySettingLogic deliverySettingLogic;
        private readonly IBranchLogic branchLogic;

        public DeliverySettingService(
            IDeliverySettingLogic deliverySettingLogic,
            IBranchLogic branchLogic)
        {
            this.deliverySettingLogic = deliverySettingLogic;
            this.branchLogic = branchLogic;
        }

        public DeliverySettingModel SaveDeliverySetting(DeliverySettingModel setting)
        {
            var branch = branchLogic.Get();
            setting.BranchId = branch.Id;
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