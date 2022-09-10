using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using FullRangeAutoturrets.Lib;
using FullRangeAutoturrets.Lib.Logging;
using Harmony;
using UnityEngine;

namespace FullRangeAutoturrets.HarmonyPatches
{
    [HarmonyPatch(typeof(FlameTurret), nameof(FlameTurret.CheckTrigger))]
    public class FlameTurret_CheckTrigger_Patch
    {
        /// <summary>
        /// Prepare the plugin's datasource for use and check if the plugin is enabled.
        /// </summary>
        public static bool Prepare(MethodBase original)
        {
            Main.CheckBootAndInit();
            return (bool)Main.instance.Config.Get("Enabled") && (bool)Main.instance.Config.Get("FlameTurrets.Enabled");
        }
        
        /// <summary>
        /// Modifies the original code instructions to modify the turret's range.
        /// </summary>
        /// <param name="originalInstructions">Original code instructions to meddle with</param>
        /// <returns>Possibly modified code instructions to replace the original method's instructions</returns>
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> originalInstructions)
        {
            // Unfortunately, the FlameTurret's implementation of a detection range isn't as neat as an autoturret's.
            // We'll override the default flag's falsification to instead execute our own helper function that will 
            // implement our own detection algorithm and return TRUE to indicate that the turret should fire and FALSE
            // to continue default behavior.
            
            // check if mod is enabled in config
            List<CodeInstruction> codeInstructionList = new List<CodeInstruction>(originalInstructions);

            try
            {
                int flagDefinitionIndex = codeInstructionList.FindIndex(
                    (Predicate<CodeInstruction>)(x => x.opcode == OpCodes.Stloc_2)); // There are multiple but we want the first one.

                if (flagDefinitionIndex != -1)
                {
                    if (codeInstructionList[flagDefinitionIndex - 1].opcode == OpCodes.Ldc_I4_0) // ... And we check if the definition is false anyway, so
                    {
                        FieldInfo field = typeof (SingletonComponent<FlameTurretAIBrain>).GetField("Instance", BindingFlags.Static | BindingFlags.Public);
                        MethodInfo method = typeof (FlameTurretAIBrain).GetMethod("EvalTargetsInRange", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        
                        // The original code is: "var flag = false;". We want to replace it with a call to our helper function.
                        codeInstructionList[flagDefinitionIndex - 1].opcode = OpCodes.Nop; // We first disable the falsification.
                        
                        // Then we insert 3 new instructions to call our helper function:
                        // 1. Load the instance of the singleton component.
                        // 2. Pass "this" to the function as the first argument (you do this by appending the ldarg.0 without operand)
                        // 3. Call the function and append the result to the evaluation stack (the "virt" part of Callvirt)
                        codeInstructionList.InsertRange(flagDefinitionIndex - 1, (IEnumerable<CodeInstruction>) new CodeInstruction[3]
                        {
                            new CodeInstruction(OpCodes.Ldsfld, (object) field),
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(OpCodes.Callvirt, (object) method)
                        });
                    }
                }
                
            }
            catch (Exception e)
            {
                LoggingManager.Log($"Unable to patch FlameTurret detection range: {e.Message}");
            }
            
            return (IEnumerable<CodeInstruction>)codeInstructionList;
        }
    }
}