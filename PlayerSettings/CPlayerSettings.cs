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
        private static Dictionary<string, string> cached_values;                

        public CPlayerSettings(int _userid)
        {
            userid = _userid;
            cached_values = new Dictionary<string, string>();
        }

        public string GetValue(string param, string default_value)
        {
            string value;
            if(!cached_values.TryGetValue(param, out value))
            {
                value = Storage.GetUserSettingValue(userid, param, default_value);
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


    }
}
