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
        CPlayerSettings[] settings;
        public SettingsApi()
        {
            settings = Array.Empty<CPlayerSettings>();
        }

        private CPlayerSettings FindUser(CCSPlayerController player)
        {
            foreach (var item in this.settings)
            {
                if (item.EqualPlayer(player))
                {
                    //Console.WriteLine($"Returned found: {item.UserId()}");
                    return item;
                }
            }            
            var newInst = new CPlayerSettings(player);
            AddPlayerInst(newInst);
            return newInst;
        }

        private void AddPlayerInst(CPlayerSettings inst)
        {
            Array.Resize(ref settings, settings.Length + 1);
            settings[settings.Length - 1] = inst;
        }


        public string GetPlayerSettingsValue(CCSPlayerController player, string param, string default_value)
        {            
            return FindUser(player).GetValue(param, default_value);            
        }
                

        public void SetPlayerSettingsValue(CCSPlayerController player, string param, string value)
        {
            FindUser(player).SetValue(param, value);
        }

        internal void LoadOnConnect(CCSPlayerController player)
        {
            var user = FindUser(player);

            Storage.LoadSettings(user.UserId(), user.ParseLoadedSettings);
        }

    }
}
