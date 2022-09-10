using Harmony;
using System;
using System.Reflection;
using UnityEngine;

namespace FullRangeAutoturrets.HarmonyPatches
{
    [HarmonyPatch(typeof (ServerMgr), "UpdateServerInformation")]
    internal class ServerMgr_UpdateServerInformation
    {
        /// <summary>
        /// ServerMgr's GameTags property, used for getting and setting the game's tags.
        /// </summary>
        private static readonly PropertyInfo GameTags = AccessTools.TypeByName("Steamworks.SteamServer").GetProperty(nameof (GameTags), BindingFlags.Static | BindingFlags.Public);
        
        /// <summary>
        /// Prepare the plugin's datasource for use and check if the plugin is enabled.
        /// </summary>
        public static bool Prepare()
        {
            Main.CheckBootAndInit();
            return (bool)Main.instance.Config.Get("Enabled");
        }
        
        /// <summary>
        /// Method to make sure that if our plugin is loaded, the server will be marked as modded to make sure the server won't be blacklisted
        /// </summary>
        [HarmonyPostfix]
        internal static void Postfix()
        {
            
            // check if mod is enabled in config
            if (!(bool)Main.instance.Config.Get("Enabled"))
            {
                // Mod is disabled in config, skipping patch
                return;
            }  
            
            try
            {
                // Get game tags and, if no modded flag is set, set it
                string strGameTags = ServerMgr_UpdateServerInformation.GetGameTags();
                if (!strGameTags.Contains(",modded"))
                    ServerMgr_UpdateServerInformation.SetGameTags(strGameTags + ",modded");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        
        /// <summary>
        /// Fetches game tags from the ServerMgr
        /// </summary>
        /// <returns>String containing game tags comma-separated</returns>
        private static string GetGameTags() => ServerMgr_UpdateServerInformation.GameTags.GetValue((object) null) as string;
        
        /// <summary>
        /// Sets new game tags in the ServerMgr
        /// </summary>
        /// <param name="value">Comma-separated string containing game tags</param>
        private static void SetGameTags(string value) => ServerMgr_UpdateServerInformation.GameTags.SetValue((object) null, (object) value);
    }
}