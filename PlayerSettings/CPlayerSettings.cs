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
        private readonly Dictionary<string, string> pending_updates;
        private readonly TaskCompletionSource<int> userIdReady;

        public CPlayerSettings(CCSPlayerController _player)
        {
            player = _player;
            userid = -1;
            cached_values = new Dictionary<string, string>();
            pending_updates = new Dictionary<string, string>();
            userIdReady = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
            Storage.GetUserIdAsync(player, (userid) =>
            {
                this.userid = userid;
                userIdReady.TrySetResult(userid);
                FlushPendingUpdates();
            });
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
            if (userid == -1)
            {
                pending_updates[param] = value;
                return;
            }

            Storage.SetUserSettingValue(userid, param, value);
        }

        public int UserId()
        {
            return userid;
        }

        public Task<int> WaitForUserIdAsync()
        {
            if (userid != -1)
                return Task.FromResult(userid);

            return userIdReady.Task;
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

            var actionsSnapshot = actions.ToArray();
            foreach (var action in actionsSnapshot)
                Server.NextFrameAsync(() => action(player));
        }

        private void FlushPendingUpdates()
        {
            if (pending_updates.Count == 0)
                return;

            foreach (var entry in pending_updates)
                Storage.SetUserSettingValue(userid, entry.Key, entry.Value);

            pending_updates.Clear();
        }

    }
}
