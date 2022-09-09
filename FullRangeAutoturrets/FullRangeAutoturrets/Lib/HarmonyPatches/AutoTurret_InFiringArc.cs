using FullRangeAutoturrets.Lib.Logging;
using Harmony;
using UnityEngine;

namespace FullRangeAutoturrets.HarmonyPatches
{
    [HarmonyPatch(typeof(AutoTurret), nameof(AutoTurret.InFiringArc))]
    public class AutoTurret_InFiringArc
    {
        /// <summary>
        /// Prepare the plugin's datasource for use
        /// </summary>
        [HarmonyPrepare]
        public static void Prepare() => Main.CheckBootAndInit();
        
        /// <summary>
        /// Patch the AutoTurret.InFiringArc method to allow turrets to detect targets outside of their normal firing arc (or even block it)
        /// </summary>
        /// <param name="__result">The result reference used to "spoof" the result</param>
        /// <param name="__instance">The autoturret being modified</param>
        /// <param name="potentialtarget">BaseCombatEntity object that is detected</param>
        /// <returns>A bool to indicate whether or not the original code should execute after our modifications</returns>
        static bool Prefix(ref bool __result, ref AutoTurret __instance, BaseCombatEntity potentialtarget)
        {
            // check if mod is enabled in config
            if (!(bool)Main.instance.Config.Get("Enabled") || !(bool)Main.instance.Config.Get("AutoTurrets.Enabled"))
            {
                return true;
            }
            
            float detectRange =
                Mathf.Clamp((float)Main.instance.Config.Get("AutoTurrets.DetectRange"), 0f, 360f);

            if (detectRange == 0f) __result = false; // If detection range is 0, turret cannot detect anything
            else if (detectRange == 360f) __result = true; // If detection range is 360, turret can detect anything
            else __result = (double) Mathf.Abs(__instance.AngleToTarget(potentialtarget)) <= (double) detectRange / 2.0; // If detection range is between the configured range, check if target is within range

            return false; // Skips the original method
        }
    }
}