using AnyBaseLib;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSettings
{
    internal static class Storage
    {
        private static IAnyBase db;

        public static void Init()
        {
            var is_sqlite = PlayerSettingsCore.plugin.Config.DatabaseParams.IsDefault();
            if (is_sqlite)
            {
                db = CAnyBase.Base("sqlite");
                db.Set(AnyBaseLib.Bases.CommitMode.AutoCommit, Path.Combine(PlayerSettingsCore.plugin.ModuleDirectory, "settings"));
            }
            else
            {
                db = CAnyBase.Base("mysql");
                db.Set(AnyBaseLib.Bases.CommitMode.AutoCommit, PlayerSettingsCore.plugin.Config.DatabaseParams.Name, PlayerSettingsCore.plugin.Config.DatabaseParams.Host, PlayerSettingsCore.plugin.Config.DatabaseParams.User, PlayerSettingsCore.plugin.Config.DatabaseParams.Password);
            }

            db.Init();
            db.QueryAsync("CREATE TABLE IF NOT EXISTS `settings_users` (`id` INTEGER PRIMARY KEY AUTO_INCREMENT, `steam` VARCHAR(255) NOT NULL)", null, (_) =>
            {
                db.QueryAsync("CREATE TABLE IF NOT EXISTS `settings_values` (`user_id` INT, `param` VARCHAR(255) NOT NULL, `value` VARCHAR(255) NOT NULL)", null, (_) =>
                {
                    if (!is_sqlite) Migrate.Init(db);
                }, true);
            },true);

            
        }
        /*
        public static int GetUserId(CCSPlayerController player)
        {
            var steamid = player.SteamID;
            var res = db.Query("select \"id\" from \"settings_users\" where \"steam\" = \"{ARG}\"", new List<string>([steamid.ToString()]));
            if(res.Count == 0)
            {
                db.Query("insert into \"settings_users\" (\"steam\") values (\"{ARG}\")", new List<string>([steamid.ToString()]), true);
                res = db.Query("select \"id\" from \"settings_users\" where \"steam\" = \"{ARG}\"", new List<string>([steamid.ToString()]));
            }
            return int.Parse(res[0][0]);
        }*/

        public static void GetUserIdAsync(CCSPlayerController player, Action<int> callback)
        {
            var steamid = player.SteamID;
            db.QueryAsync("SELECT `id` FROM `settings_users` WHERE `steam` = '{ARG}'", new List<string>([steamid.ToString()]), (data) => {
                if (data.Count > 0)
                {
                    callback(int.Parse(data[0][0]));
                }
                else
                    db.QueryAsync("INSERT INTO `settings_users` (`steam`) VALUES ('{ARG}')", new List<string>([steamid.ToString()]), (data) => GetUserIdAsync(player, callback), true);
            });
            
            
        }

        internal static void LoadSettings(int userid, Action<List<List<string>>> action)
        {
            db.QueryAsync("SELECT `param`, `value` FROM `settings_values` WHERE `user_id` = {ARG}", new List<string>([userid.ToString()]), action);
        }

        
        /*
        public static string GetUserSettingValue(int userid, string param, string default_value)
        {
            var res = db.Query("SELECT `value` FROM `settings_values` WHERE `user_id` = {ARG} AND `param` = '{ARG}'", new List<string>([userid.ToString(), param]));
            if (res.Count == 0)
            {
                db.Query("INSERT INTO `settings_values` (`user_id`, `param`, `value`) VALUES ({ARG}, '{ARG}', '{ARG}')", new List<string>([userid.ToString(), param, default_value]), true);
                return default_value;
            }
            return res[0][0];
        }*/

        public static void SetUserSettingValue(int userid, string param, string value)
        {
            db.QueryAsync("SELECT `value` FROM `settings_values` WHERE `user_id` = {ARG} AND `param` = '{ARG}'", new List<string>([userid.ToString(), param]), (data) => SetUserSettingValuePost(userid, param, value, data.Count));
        }

        private static void SetUserSettingValuePost(int userid, string param, string value, int co)
        {
            if (co == 0)
                db.QueryAsync("INSERT INTO `settings_values` (`user_id`, `param`, `value`) VALUES ({ARG}, '{ARG}', '{ARG}')", new List<string>([userid.ToString(), param, value]), null, true);
            else
                db.QueryAsync("UPDATE `settings_values` SET `value` = '{ARG}' WHERE `user_id` = {ARG} AND `param` = '{ARG}'", new List<string>([value, userid.ToString(), param]), null, true);
        }

        public static void Close()
        {
            db.Close();
        }

        
    }

}
