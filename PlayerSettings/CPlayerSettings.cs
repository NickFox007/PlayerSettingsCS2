using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSettings
{
    internal class CPlayerSettings
    {
        private int userid;
        private CCSPlayerController player;
        private Dictionary<string, string> cached_values;                

        public CPlayerSettings(CCSPlayerController _player)
        {
            player = _player;
            userid = -1;
            Storage.GetUserIdAsync(player, (userid) => this.userid = userid);
            cached_values = new Dictionary<string, string>();
        }

        public string GetValue(string param, string default_value)
        {
            string value;
            if(!cached_values.TryGetValue(param, out value) || value == null)
            {              
                value = default_value;
                cached_values[param] = value;
            }
            
            return value;
        }

        public void SetValue(string param, string value)
        {
            cached_values[param] = value;
            Storage.SetUserSettingValue(userid, param, value);
        }

        public int UserId()
        {
            return userid;
        }

        public bool EqualPlayer(CCSPlayerController _player)
        {
            return player == _player;
        }

        internal void ParseLoadedSettings(List<List<string>> rows, List<Action<CCSPlayerController>> actions)
        {
            foreach (var row in rows)
            {
                cached_values[row[0]] = row[1];
            }
            foreach (var action in actions)
                Server.NextFrameAsync(() => action(player));
        }

    }
}
