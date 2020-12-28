using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class IntegrationSystemService
    {
        private readonly IRepository<IntegrationSystemModel> repository;
        
        public IntegrationSystemService(IRepository<IntegrationSystemModel> repository)
        {
            this.repository = repository;
        }

        public IntegrationSystemModel Save(IntegrationSystemModel setting) 
        {
            IntegrationSystemModel result;

            var oldSetting = setting.Id > 0 ? null : Get(setting.BranchId);
            setting.Id = oldSetting?.Id ?? setting.Id;

            if (setting.Id > 0)
            {
                setting.Id = oldSetting.Id;

                repository.Update(setting);
                result = repository.Get(setting.Id);
            }
            else
            {
                result = repository.Create(setting);
            }

            return result;
        }

        public IntegrationSystemModel Get(int branchId)
        {
            var result = repository.Get(p => p.BranchId == branchId).FirstOrDefault();

            return result;
        }
    }
}