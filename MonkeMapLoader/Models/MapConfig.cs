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

        [JsonProperty(PropertyName = "slowJumpLimit")]
        public float SlowJumpLimit = 6.5f;

        [JsonProperty(PropertyName = "fastJumpLimit")]
        public float FastJumpLimit = 8.5f;

		[JsonProperty(PropertyName = "slowJumpMultiplier")]
        public float SlowJumpMultiplier = 1.1f;

		[JsonProperty(PropertyName = "fastJumpMultiplier")]
        public float FastJumpMultiplier = 1.3f;
    }
}