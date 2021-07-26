using Newtonsoft.Json;
using UnityEngine;

namespace VmodMonkeMapLoader.Models
{
    public class MapConfig
    {
        [JsonProperty(PropertyName = "imagePath")]
        public string ImagePath { get; set; }

        [JsonProperty(PropertyName = "cubemapImagePath")]
        public string CubemapImagePath { get; set; }

        [JsonProperty(PropertyName = "spawnPoints")]
        public string[] SpawnPoints { get; set; }

        [JsonProperty(PropertyName = "mapColor")]
        public Color MapColor { get; set; }

        [JsonProperty(PropertyName = "guid")]
        public string GUID { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }

        [JsonProperty(PropertyName = "requiredPCModIDs")]
        public string[] RequiredPCModIDs { get; set; }

        [JsonProperty(PropertyName = "requiredModIDs")]
        public string[] RequiredQuestModIDs { get; set; }
    }
}