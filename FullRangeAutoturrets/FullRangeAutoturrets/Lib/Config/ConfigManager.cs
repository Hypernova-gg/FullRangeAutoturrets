using System;
using System.IO;
using System.Reflection;
using FullRangeAutoturrets.Config;
using FullRangeAutoturrets.Lib.Logging;
using Newtonsoft.Json;
using UnityEngine;

namespace FullRangeAutoturrets.Lib.Config
{
    public class ConfigManager
    {
        
        /// <summary>
        /// Cached configuration file data
        /// </summary>
        private ConfigFile Configuration;
        
        /// <summary>
        /// Path to the configuration file
        /// </summary>
        private string ConfigPath;
        
        /// <summary>
        /// Action event that is invoked when the configuration is loaded
        /// </summary>
        public event Action OnConfigLoaded;
        
        /// <summary>
        /// Sets the config path on initialization
        /// </summary>
        public ConfigManager() {
            this.ConfigPath = $"HarmonyMods_Data/{Main.ModShortName}/Configuration.json";
        }
        
        /// <summary>
        /// Gets a value from the configuration file.
        /// </summary>
        /// <param name="propName">Name of the property to fetch. Can be nested like "group.prop1".</param>
        /// <param name="src">Source object to fetch property from. Is mainly used for nesting, when invoking this should be left null.</param>
        /// <returns>A type agnostic object, to be later typecasted</returns>
        public object Get(string propName, object src = null)
        {
            try
            {
                if (src == null) src = Configuration;
                if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");
		
                if(propName.Contains("."))//complex type nested
                {
                    var temp = propName.Split(new char[] { '.' }, 2);
                    return Get(temp[1],Get(temp[0], src));
                }
                else
                {
                    var prop = src.GetType().GetProperty(propName);
                    return prop != null ? prop.GetValue(src, null) : null;
                }
            }
            catch
            {
            }
            
            LoggingManager.Log($"Failed to get configuration value for key {propName}");
            return null;
        }
        
        /// <summary>
        /// Attempts to load the configuration file into cache and parse using JSON
        /// </summary>
        public void Load()
        {
            try
            {
                this.Configuration = JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(this.ConfigPath)) ?? new ConfigFile();
            }
            catch
            {
                LoggingManager.Log("Failed to load configuration file, loading default configuration.");
                this.Configuration = new ConfigFile();
                if (File.Exists(this.ConfigPath))
                    return;
            }
            this.Save();
            
            if(!(bool)this.Get("Enabled")) {
                LoggingManager.Log($"Mod is disabled, please enable it in the configuration file.");
            } else {
                this.OnConfigLoaded?.Invoke();
            }
        }
        
        /// <summary>
        /// Saves the configuration file to disk
        /// </summary>
        private void Save()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(this.ConfigPath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();
                
                File.WriteAllText(this.ConfigPath, JsonConvert.SerializeObject((object) this.Configuration, Formatting.Indented));
            }
            catch (Exception ex)
            {
                LoggingManager.Log((object) "Could not write to configuration file");
                Debug.LogException(ex);
            }
        }

    }
}