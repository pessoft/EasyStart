using EasyStart.Models;
using System.Collections.Generic;

namespace EasyStart.Logic.Services.GeneralSettings
{
    public interface IGeneralSettingsLogic: IBranchRemoval
    {
        SettingModel SaveSetting(SettingModel setting);
        IEnumerable<SettingModel> Get();
    }
}
