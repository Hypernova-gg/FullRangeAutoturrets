using Harmony;
using System.Reflection;
using FullRangeAutoturrets.Lib.Commands;
using FullRangeAutoturrets.Lib.Config;
using FullRangeAutoturrets.Lib.Logging;
using UnityEngine;

namespace FullRangeAutoturrets
{
    internal class Main
    {
        public static Main instance;
        
        public bool isLoaded = false;
        public bool isInitialized = false;
        public ConfigManager Config;
        public CommandManager Commands;
        public const string ModName = "Full Range Autoturrets";
        public const string ModShortName = "FullRangeAutoturrets"; // Used for paths and command prefixes and such
        public static string ModVersion;
        public const string ModAuthor = "Airathias";
        
        public void OnConfigurationLoaded()
        {
            // Bootstrap of Harmony mod completed
            LoggingManager.Log("Mod successfully loaded and configured");
            Main.instance.isInitialized = true;
            Main.instance.Commands.RegisterRCON("reload", this.OnReloadCommand, CommandFlag.BlockExecution | CommandFlag.IncludePrefix);
        }

        public void OnReloadCommand(object sender, object[] args)
        {
            Main.instance.Commands.Reset();
            Main.instance.Config.Load();
            LoggingManager.Log("Config reloaded");
        }
        
        public void Boot()
        {
            try
            {
                Main.ModVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                Debug.LogWarning((object)("[Harmony] Loaded: " + Main.ModName + " v" + Main.ModVersion + " by " +
                                          Main.ModAuthor));
                
                Main.instance.isLoaded = true;

                Main.instance.Config = new ConfigManager();
                Main.instance.Config.OnConfigLoaded += OnConfigurationLoaded;
                Main.instance.Commands = new CommandManager();

                Main.instance.Config
                    .Load(); // Do this last so that the OnConfigurationLoaded event is fired after everything is loaded
            }
            catch (System.Exception ex)
            {
                Debug.LogError((object)("[Harmony] " + Main.ModName + " failed to load: " + ex.Message));
                Debug.LogError(ex.StackTrace);
            }
        }
        
        public static void CheckBootAndInit()
        {
            if(Main.instance == null)
                Main.instance = new Main(); 
                
            if(!Main.instance.isLoaded)  
                Main.instance.Boot();
        }
    }
}