using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HarmonyTests.Lib
{
    public class Helpers
    {
        public static T GetFieldValue<T>(object obj, string name) {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            return (T)field?.GetValue(obj);
        }
        
        public static void SetFieldValue(object obj, string name, object value) {
            // Set the flags so that private and public fields from instances will be found
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var field = obj.GetType().GetField(name, bindingFlags);
            field?.SetValue(obj, value);
        }
        
        public static string Sanitize(string input)
        {
            return new string(input.Where(c => !char.IsControl(c)).ToArray());
        }
        
        private static Int32 TimeNow()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }
        
        private static BasePlayer FindPlayer(string partialNameOrID) => BasePlayer.allPlayerList.FirstOrDefault<BasePlayer>((BasePlayer x) => x.displayName.Equals(partialNameOrID, StringComparison.OrdinalIgnoreCase)) ??
                                                                        BasePlayer.allPlayerList.FirstOrDefault<BasePlayer>((BasePlayer x) => x.displayName.Contains(partialNameOrID, CompareOptions.OrdinalIgnoreCase)) ??
                                                                        BasePlayer.allPlayerList.FirstOrDefault<BasePlayer>((BasePlayer x) => x.UserIDString == partialNameOrID);

    }
}