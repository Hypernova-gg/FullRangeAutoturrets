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
        /// <summary>
        /// Instance of Main to be used by other classes.
        /// </summary>
        public static Main instance;
        
        /// <summary>
        /// Is the mod itself loaded into memory?
        /// </summary>
        public bool isLoaded = false;
        
        /// <summary>
        /// Have we initialized the mod and loaded all the required data?
        /// </summary>
        public bool isInitialized = false;
        
        /// <summary>
        /// Configuration manager to read the .JSON file.
        /// </summary>
        public ConfigManager Config;
        
        /// <summary>
        /// Command manager to manage custom commands with
        /// </summary>
        public CommandManager Commands;
        
        /// <summary>
        /// Name of the mod, only used in printed messages
        /// </summary>
        public const string ModName = "Full Range Autoturrets";
        
        /// <summary>
        /// Shortname, often ToLower'ed and used for file names and command prefixes
        /// </summary>
        public const string ModShortName = "FullRangeAutoturrets";
        
        /// <summary>
        /// Assembly version of the mod, used for version checking and printed messages
        /// </summary>
        public static string ModVersion;
        
        /// <summary>
        /// The author of the mod, used for printed messages
        /// </summary>
        public const string ModAuthor = "Airathias";
        
        /// <summary>
        /// Event action to be called when the mod is initialized and ready to be used
        /// </summary>
        public void OnConfigurationLoaded()
        {
            // Bootstrap of Harmony mod completed
            LoggingManager.Log("Mod successfully loaded and configured");
            Main.instance.isInitialized = true;
            Main.instance.Commands.RegisterRCON("reload", this.OnReloadCommand, CommandFlag.BlockExecution | CommandFlag.IncludePrefix);
        }
        
        /// <summary>
        /// Event action to be called when the reload command is executed
        /// </summary>
        /// <param name="sender">Usually a player object if it's a Console or Chat command</param>
        /// <param name="args">All command arguments</param>
        public void OnReloadCommand(object sender, object[] args)
        {
            Main.instance.Commands.Reset();
            Main.instance.Config.Load();
            LoggingManager.Log("Config reloaded");
        }
        
        /// <summary>
        /// The main entry point of the mod, called by the game when the mod is loaded
        /// </summary>
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
        
        /// <summary>
        /// Method to check if the mod is loaded, and if not, boot the instance of the mod
        /// </summary>
        public static void CheckBootAndInit()
        {
            if(Main.instance == null)
                Main.instance = new Main(); 
                
            if(!Main.instance.isLoaded)  
                Main.instance.Boot();
        }
    }
}