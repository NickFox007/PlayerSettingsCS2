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
        
        public static void Init(BasePlugin plugin, string driver = "sqlite")
        {
            db = CAnyBase.Base(driver);
            db.Set(AnyBaseLib.Bases.CommitMode.AutoCommit, Path.Combine(plugin.ModuleDirectory,"menusets"));
            
            db.Init();            
            db.Query("create table if not exists \"users\" (\"id\" integer primary key AUTOINCREMENT, \"steam\" varchar(255) not null)", null, true);
            db.Query("create table if not exists \"settings\" (\"user_id\" int, \"param\" varchar(255) not null, \"value\" varchar(255) not null)", null, true);
        }

        public static int GetUserId(CCSPlayerController player)
        {
            var steamid = player.SteamID;
            var res = db.Query("select \"id\" from users where \"steam\" = \"{ARG}\"", new List<string>([steamid.ToString()]));
            if(res.Count == 0)
            {
                db.Query("insert into \"users\" (\"steam\") values (\"{ARG}\")", new List<string>([steamid.ToString()]), true);
                res = db.Query("select \"id\" from \"users\" where \"steam\" = \"{ARG}\"", new List<string>([steamid.ToString()]));
            }
            return int.Parse(res[0][0]);
        }

        public static string GetUserSettingValue(int userid, string param, string default_value)
        {
            var res = db.Query("select \"value\" from \"settings\" where \"user_id\" = {ARG} and \"param\" = \"{ARG}\"", new List<string>([userid.ToString(), param]));
            if (res.Count == 0)
            {
                db.Query("insert into \"settings\" (\"user_id\", \"param\",\"value\") values ({ARG},\"{ARG}\", \"{ARG}\")", new List<string>([userid.ToString(), param, default_value]), true);
                return default_value;
            }
            return res[0][0];
        }

        public static void SetUserSettingValue(int userid, string param, string value)
        {
            db.Query("update \"settings\" set \"value\" = \"{ARG}\" where \"user_id\" = {ARG} and \"param\" = \"{ARG}\"", new List<string>([value, userid.ToString(), param]), true);
        }

        


    }
}
