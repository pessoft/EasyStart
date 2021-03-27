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
        private readonly IRepository<DeliverySettingModel> repository;

        public DeliverySettingService(IRepository<DeliverySettingModel> repository)
        {
            this.repository = repository;
        }

        public DeliverySettingModel GetByBranchId(int branchId)
        {
            var setting = repository.Get(p => p.BranchId == branchId).FirstOrDefault();

            return setting;
        }
    }
}