using EasyStart.Logic.Services.RepositoryFactory;
using EasyStart.Models;
using EasyStart.Repository;
using System.Collections.Generic;
using System.Linq;

namespace EasyStart.Logic.Services.GeneralSettings
{
    public class GeneralSettingsLogic : IGeneralSettingsLogic
    {
        private readonly IRepository<SettingModel, int> settingRepository;
        public GeneralSettingsLogic(IRepositoryFactory repositoryFactory)
        {
            settingRepository = repositoryFactory.GetRepository<SettingModel, int>();
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