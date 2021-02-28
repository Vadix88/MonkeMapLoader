using Newtonsoft.Json;

namespace VmodMonkeMapLoader.Models
{
    public class MapConfig
    {
        [JsonProperty(PropertyName = "rootObjectName")]
        public string RootObjectName { get; set; }
    }
}