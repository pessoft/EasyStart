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
        private readonly IDefaultEntityRepository<DeliverySettingModel> repository;
        private readonly IUniqIdEntityRepository<AreaDeliveryModel> areaDeliveryRepository;

        public DeliverySettingService(
            IDefaultEntityRepository<DeliverySettingModel> repository,
            IUniqIdEntityRepository<AreaDeliveryModel> areaDeliveryRepository)
        {
            this.repository = repository;
            this.areaDeliveryRepository = areaDeliveryRepository;
        }

        public DeliverySettingModel GetByBranchId(int branchId)
        {
            var setting = repository.Get(p => p.BranchId == branchId).FirstOrDefault();
            if (setting != null)
                setting.AreaDeliveries = GetAreaDeliveris(setting.Id);

            return setting;
        }

        public List<AreaDeliveryModel> GetAreaDeliveris(int delvierySettingId)
        {
            return areaDeliveryRepository
                    .Get(p => p.DeliverySettingId == delvierySettingId)
                    .ToList();
        }

        public bool SaveAreaDeliveries(List<AreaDeliveryModel> areaDeliveries)
        {
            if (areaDeliveries == null || !areaDeliveries.Any())
                return false;

            var dict = areaDeliveries.ToDictionary(p => p.UniqId);
            var ids = dict.Keys.ToList();
            var deliverySettingId = areaDeliveries.First().DeliverySettingId;

            var allAreas = GetAreaDeliveris(deliverySettingId);
            var updates = allAreas
                    .Where(x => ids.Contains(x.UniqId))
                    .ToList();
            var newAreas = areaDeliveries
                   .Where(p => !updates.Exists(x => p.UniqId == x.UniqId))
                   .ToList();
            var removeAreas = allAreas
                .Where(x => !updates.Exists(p => p.UniqId == x.UniqId))
                .ToList();

            areaDeliveryRepository.Update(updates);
            areaDeliveryRepository.Create(areaDeliveries);
            areaDeliveryRepository.Remove(removeAreas);

            return true;
        }
    }
}