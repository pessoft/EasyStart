using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Logic.Services.Branch
{
    public class GeneralSettingsLogic : IGeneralSettingsLogic
    {
        private readonly IDefaultEntityRepository<SettingModel> settingRepository;
        public GeneralSettingsLogic(IDefaultEntityRepository<SettingModel> settingRepository)
        {
            this.settingRepository = settingRepository;
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