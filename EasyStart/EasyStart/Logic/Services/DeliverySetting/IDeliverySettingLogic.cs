using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyStart.Logic.Services.DeliverySetting
{
    public interface IDeliverySettingLogic: IBranchRemoval
    {
        DeliverySettingModel SaveDeliverySetting(DeliverySettingModel setting);
        DeliverySettingModel GetByBranchId(int branchId);
        List<AreaDeliveryModel> GetAreaDeliveris(int delvierySettingId);
        List<AreaDeliveryModel> SaveAreaDeliveries(List<AreaDeliveryModel> areaDeliveries);
    }
}
