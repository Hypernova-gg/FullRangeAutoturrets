using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using FullRangeAutoturrets.Lib.Logging;
using Harmony;
using HarmonyTests.Lib;
using UnityEngine;

namespace FullRangeAutoturrets.HarmonyPatches
{
    [HarmonyPatch(typeof(AutoTurret), nameof(AutoTurret.IdleTick))]
    public class AutoTurret_IdleTick
    {
        /// <summary>
        /// Prepare the plugin's datasource for use
        /// </summary>
        [HarmonyPrepare]
        public static void Prepare() => Main.CheckBootAndInit();
        
        /// <summary>
        /// Patch the AutoTurret.IdleTick method to customize turret animation behavior
        /// </summary>
        /// <param name="__instance">The autoturret being modified</param>
        /// <returns>A bool to indicate whether or not the original code should execute after our modifications</returns>
        static bool Prefix(AutoTurret __instance)
        {
            // check if mod is enabled in config
            if (!(bool)Main.instance.Config.Get("Enabled") || !(bool)Main.instance.Config.Get("AutoTurrets.Enabled"))
            {
                // Mod is disabled in config, skipping patch
                return true;
            }  
            
            // Fetch the turret's next idle tick timer using reflection due to the property being private
            float nextIdleAimTime = Helpers.GetFieldValue<float>(__instance, "nextIdleAimTime");
            if (nextIdleAimTime < 10f)
            {
                return true; // Let's wait until we're sure there's a proper idle time set, because this property is set pretty late
            }
            
            // Shorter notation for the original method
            if ((double) UnityEngine.Time.realtimeSinceStartup > (double) nextIdleAimTime)
            {

                float rotationRange =
                    Mathf.Clamp((float)Main.instance.Config.Get("AutoTurrets.RotationRange"), 0f, 360f);
                
                // If the rotation range is under 45 degrees, it's a pretty small range, so we'll adjust the turret's idle tick to be a bit more frequent
                nextIdleAimTime = UnityEngine.Time.realtimeSinceStartup + UnityEngine.Random.Range((rotationRange/2 < 45f ? 2f : 4f), (rotationRange/2 < 45f ? 3f : 5f));

                Vector3 targetAimDir = Quaternion.LookRotation(__instance.transform.forward, Vector3.up) * Quaternion.AngleAxis(0, Vector3.up) * Vector3.forward;
                if (rotationRange != 0)
                {
                    // Make rotations more random but still with a minimum range to make sure the turret doesn't aim in basically the same direction every time
                    float minDir = UnityEngine.Random.Range(-(rotationRange / 2), -(rotationRange > 90f ? 20f : 0f));
                    float maxDir = UnityEngine.Random.Range((rotationRange > 90f ? 20f : 0f), (rotationRange / 2));
                
                    // Set the turret's next idle tick timer and next target aim direction using reflection due to the property being private
                    targetAimDir = Quaternion.LookRotation(__instance.transform.forward, Vector3.up) * Quaternion.AngleAxis(UnityEngine.Random.Range(minDir, maxDir), Vector3.up) * Vector3.forward;
                }
                
                // We only need to set these, the original method will handle the rest
                Helpers.SetFieldValue(__instance, "targetAimDir", targetAimDir);
                Helpers.SetFieldValue(__instance, "nextIdleAimTime", nextIdleAimTime);
            }
            
            return true; // Continues to original method
        }
    }
}