using AnyBaseLib;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSettings
{
    internal static class Migrate
    {
        static IAnyBase sqlite, mysql;

        internal static void Init(IAnyBase mysql)
        {
            Migrate.mysql = mysql;
            sqlite = CAnyBase.Base("sqlite");
            sqlite.Set(AnyBaseLib.Bases.CommitMode.AutoCommit, Path.Combine(PlayerSettingsCore.plugin.ModuleDirectory, "settings"));
            sqlite.Init();

            mysql.QueryAsync($"SELECT COUNT(*) FROM `{PlayerSettingsCore.plugin.Config.DatabaseParams.Table}users`", [], StartMigrate);
        }


        private static void StartMigrate(List<List<string>> res)
        {
            if (res[0][0] == "0")
                Task.Run(MigrateUsers);
            else
                Close();
        }

        private static void MigrateUsers()
        {
            var res = sqlite.Query($"SELECT `id`,`steam` FROM `{PlayerSettingsCore.plugin.Config.DatabaseParams.Table}users`", []);
            if (res.Count == 0)
            {
                Console.WriteLine("Nothing to migrate [users]");
                Close();
            }
            else
            {
                PlayerSettingsCore.plugin.Logger.LogInformation("Migrating users...");
                var sql = "";
                var args = new List<string>();
                int count = 0;
                foreach (var row in res)
                {
                    sql += "INSERT INTO `" + PlayerSettingsCore.plugin.Config.DatabaseParams.Table + "users` (`id`,`steam`) VALUES ({ARG}, '{ARG}'); ";
                    args.Add(row[0]);
                    args.Add(row[1]);
                    count++;
                    if (count % (res.Count / 10) == 0) PlayerSettingsCore.plugin.Logger.LogInformation($"Migrating... [{Math.Round((float)count / ((float)res.Count) * 100, MidpointRounding.ToPositiveInfinity)}%]");
                }

                mysql.QueryAsync(sql, args, (_) => PlayerSettingsCore.plugin.Logger.LogInformation("Migrated users!"), true);

                MigrateSettings();
            }
        }

        private static void MigrateSettings()
        {
            var res = sqlite.Query($"SELECT `user_id`,`param`,`value` FROM `{PlayerSettingsCore.plugin.Config.DatabaseParams.Table}values`", []);
            if (res.Count > 0)
            {
                PlayerSettingsCore.plugin.Logger.LogInformation("Migrating settings...");
                var sql = "";
                var args = new List<string>();
                int count = 0;
                foreach (var row in res)
                {
                    sql += $"INSERT INTO `" + PlayerSettingsCore.plugin.Config.DatabaseParams.Table + "values` (`user_id`,`param`, `value`) VALUES ({ARG}, '{ARG}', '{ARG}'); ";
                    args.Add(row[0]);
                    args.Add(row[1]);
                    args.Add(row[2]);
                    count++;
                    if(count % (res.Count/10) == 0) PlayerSettingsCore.plugin.Logger.LogInformation($"Migrating... [{Math.Round((float)count / ((float)res.Count) * 100, MidpointRounding.ToPositiveInfinity)}%]");
                }

                mysql.QueryAsync(sql, args, (_) =>
                {
                    PlayerSettingsCore.plugin.Logger.LogInformation("Migrated settings!");
                    Close();
                }, true);

            }
            else
            {
                Console.WriteLine("Nothing to migrate [values]");
                Close();
            }
        }


        private static void Close()
        {
            sqlite.Close();
        }
    }
}
