using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSettings
{
    public interface ISettingsApi
    {
        public string GetPlayerSettingsValue(CCSPlayerController player, string param, string default_value);
        public void SetPlayerSettingsValue(CCSPlayerController player, string param, string value);
        public void AddHook(Action<CCSPlayerController> action);
        public void RemHook(Action<CCSPlayerController> action);

        // Coming soon...
        //public void RegisterTogglableSetting(string name, string viewName);
        //public void RegisterSelectingSetting(string name, string viewName, List<string> values);
        //public List<SettingItem> GetSettingItems();
    }
}
