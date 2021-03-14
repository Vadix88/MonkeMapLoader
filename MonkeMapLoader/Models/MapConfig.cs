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
    }
}