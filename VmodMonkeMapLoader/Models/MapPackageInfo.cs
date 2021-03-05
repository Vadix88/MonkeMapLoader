using Newtonsoft.Json;

namespace VmodMonkeMapLoader.Models
{
    public class MapPackageInfo
    {
        [JsonProperty(PropertyName = "pcFileName")]
        public string PcFileName { get; set; }

        [JsonProperty(PropertyName = "androidFileName")]
        public string AndroidFileName { get; set; }

        [JsonProperty(PropertyName = "descriptor")]
        public Descriptor Descriptor { get; set; }

        [JsonProperty(PropertyName = "config")]
        public MapConfig Config { get; set; }
    }
}