using Newtonsoft.Json;

namespace FullRangeAutoturrets.Config
{
    public class ConfigFile
    {
        [JsonProperty(PropertyName = "Modification enabled (true/false)")]
        public bool Enabled { get; set; } = true;

        [JsonProperty(PropertyName = "AutoTurret Options")]
        public AutoTurretOptions AutoTurrets { get; set; } = new AutoTurretOptions();
        
        [JsonProperty(PropertyName = "FlameTurret Options")]
        public FlameTurretOptions FlameTurrets { get; set; } = new FlameTurretOptions();

        public class AutoTurretOptions
        {
            [JsonProperty(PropertyName = "Modify all AutoTurrets (true/false)")]
            public bool Enabled { get; set; } = true;
            
            [JsonProperty(PropertyName = "Detection Range Degrees (0-360)")]
            public float DetectRange { get; set; } = 360f;
            
            [JsonProperty(PropertyName = "Rotation Range Degrees (0-360)")]
            public float RotationRange { get; set; } = 360f;
        }
        
        public class FlameTurretOptions
        {
            [JsonProperty(PropertyName = "Modify all FlameTurrets (true/false)")]
            public bool Enabled { get; set; } = true;
            
            [JsonProperty(PropertyName = "Detection Range Degrees (0-360)")]
            public float DetectRange { get; set; } = 360f;
            
            [JsonProperty(PropertyName = "Rotation Range Degrees (0-360)")]
            public float RotationRange { get; set; } = 360f;
        }
    }
}


            
