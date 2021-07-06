using EasyStart.Models;
using System.Collections.Generic;

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
