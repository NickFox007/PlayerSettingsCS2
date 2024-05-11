using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PlayerSettings
{
    
    internal class SettingsApi : ISettingsApi
    {
        List<CPlayerSettings> settings;
        public SettingsApi()
        {
            settings = new List<CPlayerSettings>();
        }

        private CPlayerSettings FindUser(int userid)
        {
            foreach (var item in this.settings)
            {
                if (item.UserId() == userid)
                {
                    return item;
                }
            }
            var newInst = new CPlayerSettings(userid);
            settings.Add(newInst);
            return newInst;
        }

        public string GetPlayerSettingsValue(CCSPlayerController player, string param, string default_value)
        {            
            return FindUser(Storage.GetUserId(player)).GetValue(param, default_value);            
        }

        public void SetPlayerSettingsValue(CCSPlayerController player, string param, string value)
        {
            FindUser(Storage.GetUserId(player)).SetValue(param, value);
        }
    }
}
