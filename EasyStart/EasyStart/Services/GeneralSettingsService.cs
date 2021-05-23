using EasyStart.Logic.Services.Branch;
using EasyStart.Logic.Services.GeneralSettings;
using EasyStart.Models;
using EasyStart.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EasyStart.Services
{
    public class GeneralSettingsService
    {
        private readonly IGeneralSettingsLogic generalSettingsLogic;
        private readonly IBranchLogic branchLogic;
        private GeneralSettingsLogic generalSettingLogic;

        public GeneralSettingsService(
            IGeneralSettingsLogic generalSettingsLogic,
            IBranchLogic branchLogic)
        {
            this.generalSettingsLogic = generalSettingsLogic;
            this.branchLogic = branchLogic;
        }

        public GeneralSettingsService(GeneralSettingsLogic generalSettingLogic, BranchLogic branchLogic)
        {
            this.generalSettingLogic = generalSettingLogic;
            this.branchLogic = branchLogic;
        }

        public SettingModel SaveSettings(SettingModel setting)
        {
            var branch = branchLogic.Get();
            setting.BranchId = branch.Id;

            return generalSettingsLogic.SaveSetting(setting);
        }

    }
}