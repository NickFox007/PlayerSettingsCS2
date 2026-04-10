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
        private readonly Dictionary<int, CPlayerSettings> settingsBySlot;
        internal List<Action<CCSPlayerController>> actions = new List<Action<CCSPlayerController>>();
        public SettingsApi()
        {
            settingsBySlot = new Dictionary<int, CPlayerSettings>();
            if (actions == null)
                actions = new List<Action<CCSPlayerController>>();
            else
                actions.Clear();
            SettingItems.Init();
        }

        public void AddHook(Action<CCSPlayerController> action)
        {
            actions.Add(action);
        }

        public void RemHook(Action<CCSPlayerController> action)
        {
            actions.RemoveAll(x => x == action);
        }

        private CPlayerSettings FindUser(CCSPlayerController player)
        {
            if (settingsBySlot.TryGetValue(player.Slot, out var existing))
            {
                if (existing.EqualPlayer(player))
                    return existing;
            }

            var newInst = new CPlayerSettings(player);
            settingsBySlot[player.Slot] = newInst;
            return newInst;
        }

        internal void RemovePlayer(int slot)
        {
            settingsBySlot.Remove(slot);
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
            _ = LoadOnConnectAsync(user);
        }

        private async Task LoadOnConnectAsync(CPlayerSettings user)
        {
            await user.WaitForUserIdAsync().ConfigureAwait(false);
            Storage.LoadSettings(user.UserId(), (vars) => user.ParseLoadedSettings(vars, actions));
        }

        public void RegisterTogglableSetting(string name, string viewName)
        {
            SettingItems.AddTogglable(name, viewName);
        }

        public void RegisterSelectingSetting(string name, string viewName, Dictionary<string, string> values)
        {
            SettingItems.AddSelecting(name, viewName, values);
        }

        public List<SettingItem> GetSettingItems()
        {
            var list = new List<SettingItem>();
            foreach (var item in SettingItems.Items)
                list.Add(item);
            return list;
        }

    }

    
}
