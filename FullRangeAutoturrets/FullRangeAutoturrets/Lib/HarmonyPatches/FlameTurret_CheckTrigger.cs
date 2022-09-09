using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using FullRangeAutoturrets.Lib.Logging;
using Harmony;
using HarmonyTests.Lib;
using UnityEngine;

namespace FullRangeAutoturrets.HarmonyPatches
{
    [HarmonyPatch(typeof(FlameTurret), nameof(FlameTurret.CheckTrigger))]
    public class FlameTurret_CheckTrigger_Patch
    {
        /// <summary>
        /// Prepare the plugin's datasource for use. This is called before the first patch is applied, and before the game is loaded.
        /// This was a cheeky way to get the plugin's datasource into the harmony patch since transpilers are usually executed before the plugin is loaded.
        /// </summary>
        [HarmonyPrepare]
        public static void Prepare() => Main.CheckBootAndInit();
        
        /// <summary>
        /// Modifies the original code instructions to modify the turret's range.
        /// </summary>
        /// <param name="originalInstructions">Original code instructions to meddle with</param>
        /// <returns>Possibly modified code instructions to replace the original method's instructions</returns>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> originalInstructions)
        {
            // Unfortunately, the FlameTurret's implementation of a detection range isn't as neat as an autoturret's.
            // We have to manipulate an IF statement within its' AI think tick method.
            
            // check if mod is enabled in config
            List<CodeInstruction> codeInstructionList = new List<CodeInstruction>(originalInstructions);

            try
            {
                if (!Main.instance.isLoaded || !(bool)Main.instance.Config?.Get("Enabled") ||
                    !(bool)Main.instance.Config?.Get("FlameTurrets.Enabled"))
                {
                    return codeInstructionList;
                }

                float detectionRange =
                    Mathf.Clamp((float)Main.instance.Config.Get("FlameTurrets.DetectRange"), 0f, 360f);
                
                // The original method detects a player within 0.5 degrees of the turret's forward vector, so we need to
                // change the 0.5f to our new detection range.
                int index1 = codeInstructionList.FindIndex(
                    (Predicate<CodeInstruction>)(x => x.opcode == OpCodes.Ldc_R4 && (float)x.operand == 0.5f));

                if (index1 == -1)
                {
                    LoggingManager.Log($"Unable to find location in source code to patch. Aborting patch.");
                    return (IEnumerable<CodeInstruction>)codeInstructionList;
                }
                    
                codeInstructionList[index1].operand =
                    (float)detectionRange; // Modify the value of the Ldc_R4 instruction
            }
            catch (Exception e)
            {
                LoggingManager.Log($"Unable to patch FlameTurret detection range: {e.Message}");
            }

            return (IEnumerable<CodeInstruction>)codeInstructionList;
        }
    }
}