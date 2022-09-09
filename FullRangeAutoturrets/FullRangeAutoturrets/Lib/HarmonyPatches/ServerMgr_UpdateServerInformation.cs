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
        private static readonly PropertyInfo GameTags = typeof (ServerMgr).GetProperty("GameTags",
            BindingFlags.Instance | BindingFlags.NonPublic);
        
        /// <summary>
        /// Prepare the plugin's datasource for use
        /// </summary>
        [HarmonyPrepare]
        public static void Prepare() => Main.CheckBootAndInit();
        
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
                string gameTags = ServerMgr_UpdateServerInformation.GameTags.GetValue((object) null) as string;
                if (gameTags.Contains(",modded"))
                    return;
                ServerMgr_UpdateServerInformation.GameTags.SetValue((object) null, gameTags + ",modded");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}