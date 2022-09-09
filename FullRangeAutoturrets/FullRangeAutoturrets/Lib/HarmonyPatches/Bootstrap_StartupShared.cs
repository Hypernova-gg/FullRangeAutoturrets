using System.Reflection;
using Harmony;
using UnityEngine;

namespace FullRangeAutoturrets.HarmonyPatches
{
    [HarmonyPatch(typeof(Bootstrap), "StartupShared")]
    internal class Bootstrap_StartupShared
    {
        /// <summary>
        /// Prepare the plugin's datasource for use
        /// </summary>
        [HarmonyPrefix]
        private static bool Prefix()
        {
            // Shouldn't be needed, but just in case
            Main.CheckBootAndInit();
            return true;
        }
    }
}