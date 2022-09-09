using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FullRangeAutoturrets.Lib.Logging
{
    public class LoggingManager
    {

        public static void Log(object arg)
        {
            // if arg is string, log it, else dump it
            if (arg is string || arg is int || arg is float || arg is double || arg is bool)
            {
                Debug.Log((object)(Main.ModName + ": " + arg.ToString()));
            }
            else
            {
                Debug.Log((object)(Main.ModName + ": " + arg.GetType().Name));
                LoggingManager.Dump(arg, true, Main.ModName);
            }

        }
        
        public static void Dump(object obj, bool dumpProps = false, string prefix = "DUMP")
        {
            if (obj == null)
            {
                Debug.Log((object)($"[{prefix}] NULL"));
                return;
            }
            
            Debug.Log((object)($"[{prefix}] Hash: {obj.GetHashCode().ToString()} | Type: {obj.GetType().ToString()}"));

            if (dumpProps)
            {
                var props = GetProperties(obj);

                if (props.Count > 0)
                {
                    Debug.Log((object)("-------------------------"));
                }

                foreach (var prop in props)
                {
                    Debug.Log((object)($"{prop.Key}: {prop.Value}"));
                }
            }
        }
        private static Dictionary<string, string> GetProperties(object obj)
        {
            var props = new Dictionary<string, string>();
            if (obj == null)
                return props;

            var type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                var val = prop.GetValue(obj, new object[] { });
                var valStr = val == null ? "" : val.ToString();
                props.Add(prop.Name, valStr);
            }

            return props;
        }

    }
}