using Newtonsoft.Json;

namespace VmodMonkeMapLoader.Models
{
    public class MapDescriptor
    {
        [JsonProperty(PropertyName = "objectName")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "author")]
        public string Author { get; set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }
    }
}