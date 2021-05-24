using EasyStart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EasyStart.Logic.Services.GeneralSettings
{
    public interface IGeneralSettingsLogic: IBranchRemoval
    {
        SettingModel SaveSetting(SettingModel setting);
        IEnumerable<SettingModel> Get();
    }
}
