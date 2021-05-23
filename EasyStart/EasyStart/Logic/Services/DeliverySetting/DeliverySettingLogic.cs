﻿using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.DeliverySetting
{
    public class DeliverySettingLogic: IDeliverySettingLogic
    {
        private readonly IBaseRepository<DeliverySettingModel, int> repository;
        private readonly IBaseRepository<AreaDeliveryModel, string> areaDeliveryRepository;

        public DeliverySettingLogic(
            IBaseRepository<DeliverySettingModel, int> repository,
            IBaseRepository<AreaDeliveryModel, string> areaDeliveryRepository)
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

            var dict = areaDeliveries.ToDictionary(p => p.Id);
            var ids = dict.Keys.ToList();
            var deliverySettingId = areaDeliveries.First().DeliverySettingId;

            var allAreas = GetAreaDeliveris(deliverySettingId);
            var updates = allAreas
                    .Where(x => ids.Contains(x.Id))
                    .Select(p => dict[p.Id])
                    .ToList();
            var newAreas = areaDeliveries
                   .Where(p => !updates.Exists(x => p.Id == x.Id))
                   .ToList();
            var removeAreas = allAreas
                .Where(x => !updates.Exists(p => p.Id == x.Id))
                .ToList();

            areaDeliveryRepository.Update(updates);
            areaDeliveryRepository.Create(newAreas);
            areaDeliveryRepository.Remove(removeAreas);

            return GetAreaDeliveris(deliverySettingId);
        }

        public void RemoveByBranch(int branchId)
        {
            var setting = repository.Get(p => p.BranchId == branchId).FirstOrDefault();

            if (setting != null)
            {
                setting.IsDeleted = true;
                repository.Update(setting);
            }
        }
    }
}