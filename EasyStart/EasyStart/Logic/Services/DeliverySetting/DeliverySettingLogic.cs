using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.DeliverySetting
{
    public class DeliverySettingLogic: IDeliverySettingLogic
    {
        private readonly IDefaultEntityRepository<DeliverySettingModel> repository;
        private readonly IUniqIdEntityRepository<AreaDeliveryModel> areaDeliveryRepository;

        public DeliverySettingLogic(
            IDefaultEntityRepository<DeliverySettingModel> repository,
            IUniqIdEntityRepository<AreaDeliveryModel> areaDeliveryRepository)
        {
            this.repository = repository;
            this.areaDeliveryRepository = areaDeliveryRepository;
        }

        public DeliverySettingModel SaveDeliverySetting(DeliverySettingModel setting)
        {
            var updateSetting = GetByBranchIdWithoutAreaDelivery(setting.BranchId);

            if (updateSetting != null)
            {
                setting.Id = updateSetting.Id;
                setting = repository.Update(setting);
            }
            else
                setting = repository.Create(setting);

            setting.AreaDeliveries.ForEach(p => p.DeliverySettingId = setting.Id);

            SaveAreaDeliveries(setting.AreaDeliveries);

            return setting;
        }

        public DeliverySettingModel GetByBranchId(int branchId)
        {
            var setting = GetByBranchIdWithoutAreaDelivery(branchId);
            if (setting != null)
                setting.AreaDeliveries = GetAreaDeliveris(setting.Id);

            return setting;
        }

        private DeliverySettingModel GetByBranchIdWithoutAreaDelivery(int branchId)
        {
            var setting = repository.Get(p => p.BranchId == branchId).FirstOrDefault();

            return setting;
        }

        public List<AreaDeliveryModel> GetAreaDeliveris(int delvierySettingId)
        {
            return areaDeliveryRepository
                    .Get(p => p.DeliverySettingId == delvierySettingId)
                    .ToList();
        }

        public List<AreaDeliveryModel> SaveAreaDeliveries(List<AreaDeliveryModel> areaDeliveries)
        {
            if (areaDeliveries == null || !areaDeliveries.Any())
                return areaDeliveries;

            var dict = areaDeliveries.ToDictionary(p => p.UniqId);
            var ids = dict.Keys.ToList();
            var deliverySettingId = areaDeliveries.First().DeliverySettingId;

            var allAreas = GetAreaDeliveris(deliverySettingId);
            var updates = allAreas
                    .Where(x => ids.Contains(x.UniqId))
                    .Select(p => dict[p.UniqId])
                    .ToList();
            var newAreas = areaDeliveries
                   .Where(p => !updates.Exists(x => p.UniqId == x.UniqId))
                   .ToList();
            var removeAreas = allAreas
                .Where(x => !updates.Exists(p => p.UniqId == x.UniqId))
                .ToList();

            areaDeliveryRepository.Update(updates);
            areaDeliveryRepository.Create(newAreas);
            areaDeliveryRepository.Remove(removeAreas);

            return GetAreaDeliveris(deliverySettingId);
        }
    }
}