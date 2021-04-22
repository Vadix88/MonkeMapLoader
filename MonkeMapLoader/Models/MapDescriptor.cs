using Newtonsoft.Json;

namespace VmodMonkeMapLoader.Models
{
    public class Descriptor
    {
        [JsonProperty(PropertyName = "objectName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "pcRequiredVersion")]
        public string PcRequiredVersion { get; set; } = "1.0.0";

        [JsonProperty(PropertyName = "androidRequiredVersion")]
        public string AndroidRequiredVersion { get; set; } = "1.0.0";
    }
}