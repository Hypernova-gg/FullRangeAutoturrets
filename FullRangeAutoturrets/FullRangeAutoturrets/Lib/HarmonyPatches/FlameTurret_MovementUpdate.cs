using System.Reflection;
using FullRangeAutoturrets.Lib.Logging;
using Harmony;
using HarmonyTests.Lib;
using UnityEngine;

namespace FullRangeAutoturrets.HarmonyPatches
{
    [HarmonyPatch(typeof(FlameTurret), nameof(FlameTurret.MovementUpdate))]
    public class FlameTurret_MovementUpdate
    {
        /// <summary>
        /// Prepare the plugin's datasource for use and check if the plugin is enabled.
        /// </summary>
        public static bool Prepare()
        {
            Main.CheckBootAndInit();
            return (bool)Main.instance.Config.Get("Enabled") && (bool)Main.instance.Config.Get("FlameTurrets.Enabled");
        }
        
        /// <summary>
        /// Modifies the turret's physical movement to allow for a larger range of movement
        /// </summary>
        /// <param name="delta">Parameter from the original method to increase rotation degrees</param>
        /// <param name="__instance">FlameTurret instance to meddle with</param>
        /// <returns>A bool to indicate whether or not the original code should execute after our modifications</returns>
        static bool Prefix(float delta, FlameTurret __instance)
        {
            float rotationRange =
                Mathf.Clamp((float)Main.instance.Config.Get("FlameTurrets.RotationRange"), 0f, 360f);

            if (rotationRange == 0f)
            {
                __instance.aimDir.y = 0f; // Stop turret from rotating
            }
            else
            {
                int turnDir = Helpers.GetFieldValue<int>(__instance, "turnDir");
                float arc = rotationRange / 2.0f;
                
                // Original method implementation
                __instance.aimDir += new Vector3(0.0f, delta * __instance.GetSpinSpeed(), 0.0f) * (float) turnDir;
                
                // If we're not rotating 360 degrees, clamp the rotation and stop the turret from spinning by changing direction
                if(rotationRange < 360f)
                {
                    if ((double) __instance.aimDir.y < (double) arc && (double) __instance.aimDir.y > -(double) arc)
                        return false;
            
                    turnDir *= -1;
                    Helpers.SetFieldValue(__instance, "turnDir", turnDir);
                    __instance.aimDir.y = Mathf.Clamp(__instance.aimDir.y, -arc, arc);
                    return false;
                }
                
                Helpers.SetFieldValue(__instance, "turnDir", 1); // Force continuous clockwise rotation
                
                // Make sure the rotation doesn't stop at 180 degrees, and instead continues to rotate at -180 degrees
                float correctedAimDir = __instance.aimDir.y >= arc ? -arc : __instance.aimDir.y; 
                __instance.aimDir.y = Mathf.Clamp(correctedAimDir, -arc, arc);
            }
            
            return false; // Skips the original method
        }
    }
}