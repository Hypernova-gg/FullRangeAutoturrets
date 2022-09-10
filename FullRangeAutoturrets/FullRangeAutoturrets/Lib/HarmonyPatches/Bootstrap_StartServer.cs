using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using FullRangeAutoturrets.Lib;
using Harmony;

namespace FullRangeAutoturrets.HarmonyPatches
{
    [HarmonyPatch(typeof (Bootstrap), "StartServer")]
    public class Bootstrap_StartServer_Patch
    {
        /// <summary>
        /// Prepare the plugin's datasource for use and check if the plugin is enabled.
        /// </summary>
        public static bool Prepare()
        {
            Main.CheckBootAndInit();
            return (bool)Main.instance.Config.Get("Enabled");
        }
        
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpile(
            IEnumerable<CodeInstruction> originalInstructions)
        {
            List<CodeInstruction> codeInstructionList = new List<CodeInstruction>(originalInstructions);
            MethodInfo method = typeof (FlameTurretAIBrain).GetMethod("Initialize", BindingFlags.Static | BindingFlags.NonPublic);
            codeInstructionList.InsertRange(0, (IEnumerable<CodeInstruction>) new CodeInstruction[1]
            {
                new CodeInstruction(OpCodes.Call, (object) method)
            });
            return (IEnumerable<CodeInstruction>) codeInstructionList;
        }
    }
}