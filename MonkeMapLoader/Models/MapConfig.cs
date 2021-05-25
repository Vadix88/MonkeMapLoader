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
        public float SlowJumpLimit = Helpers.SharedConstants.SlowJumpLimit;

        [JsonProperty(PropertyName = "fastJumpLimit")]
        public float FastJumpLimit = Helpers.SharedConstants.FastJumpLimit;

		[JsonProperty(PropertyName = "slowJumpMultiplier")]
        public float SlowJumpMultiplier = Helpers.SharedConstants.SlowJumpMultiplier;

		[JsonProperty(PropertyName = "fastJumpMultiplier")]
        public float FastJumpMultiplier = Helpers.SharedConstants.FastJumpMultiplier;

        [JsonProperty(PropertyName = "gameMode")]
        public string gameMode = "";
    }
}