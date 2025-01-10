using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerSettings
{
    internal static class SettingItems
    {
        internal static List<SettingItem> Items;

        internal static void Init()
        {
            Items = new List<SettingItem>();
        }

        internal static void AddTogglable(string name, string viewName)
        {
            DeleteIfExist(name);
            Items.Add(new SettingItem(SettingType.Togglable, name, viewName));
        }

        internal static void AddSelecting(string name, string viewName, Dictionary<string, string> values)
        {
            DeleteIfExist(name);
            Items.Add(new SettingItem(SettingType.Selecting, name, viewName, values));
        }

        internal static void DeleteIfExist(string name)
        {
            for (int i = 0; i < Items.Count; i++)
                if (Items[i].Name == name)
                {
                    Items.RemoveAt(i);
                    i--;
                }
        }
    }
}
