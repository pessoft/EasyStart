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

            var oldSetting = Get(setting.BranchId);
            if (oldSetting != null)
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