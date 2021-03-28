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
                setting.AreaDeliveries = areaDeliveryRepository
                    .Get(p => p.DeliverySettingId == setting.Id)
                    .ToList();

            return setting;
        }
    }
}