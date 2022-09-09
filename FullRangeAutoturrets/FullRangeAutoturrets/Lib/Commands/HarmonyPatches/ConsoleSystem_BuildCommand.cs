using Harmony;

namespace FullRangeAutoturrets.Lib.HarmonyPatches
{
    [HarmonyPatch(typeof (ConsoleSystem), "Run", new System.Type[] {typeof (ConsoleSystem.Option), typeof (string), typeof (object[])})]
    internal class ConsoleSystem_BuildCommand
    {
        /// <summary>
        /// Prepare the plugin's datasource for use
        /// </summary>
        [HarmonyPrepare]
        public static void Prepare() => Main.CheckBootAndInit();
        
        /// <summary>
        /// Patch the Run method to add our custom commands
        /// </summary>
        /// <param name="options">Contains the player connection (if any) and other such data</param>
        /// <param name="strCommand">Full, unfiltered command string</param>
        /// <param name="args">Basically useless since all args are in the command string, but you know. Compatibility and all that.</param>
        /// <returns>Bool to indicate whether the original code should run after command parsing and execution</returns>
        [HarmonyPrefix]
        private static bool Prefix(ConsoleSystem.Option options, string strCommand, params object[] args)
        {
            try
            {
                return Main.instance.Commands.Handler(options, strCommand, args);
            }
            catch
            {
            }
            return true;
        }
    }
}