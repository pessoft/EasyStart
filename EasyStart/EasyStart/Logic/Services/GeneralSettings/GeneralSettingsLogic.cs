using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.GeneralSettings
{
    public class GeneralSettingsLogic : IGeneralSettingsLogic
    {
        private readonly IBaseRepository<SettingModel, int> settingRepository;
        public GeneralSettingsLogic(IBaseRepository<SettingModel, int> settingRepository)
        {
            this.settingRepository = settingRepository;
        }

        public IEnumerable<SettingModel> Get()
        {
            return settingRepository.Get();
        }

        public void RemoveByBranch(int branchId)
        {
            var setting = settingRepository.Get(p => p.BranchId == branchId).FirstOrDefault();

            if (setting != null)
            {
                setting.IsDeleted = true;
                settingRepository.Update(setting);
            }
        }

        public SettingModel SaveSetting(SettingModel setting)
        {
            var oldSetting = settingRepository.Get(setting.Id);
            SettingModel newSetting = null;

            if (oldSetting != null)
                newSetting = settingRepository.Update(setting);
            else
                newSetting = settingRepository.Create(setting);

            return newSetting;
        }
    }
}