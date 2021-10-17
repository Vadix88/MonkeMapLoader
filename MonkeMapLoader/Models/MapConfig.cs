using System.Collections.Generic;
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

        [JsonProperty(PropertyName = "gravity")]
        public float Gravity { get; set; }

        [JsonProperty(PropertyName = "slowJumpLimit")]
        public float SlowJumpLimit { get; set; }

        [JsonProperty(PropertyName = "fastJumpLimit")]
        public float FastJumpLimit { get; set; }

        [JsonProperty(PropertyName = "slowJumpMultiplier")]
        public float SlowJumpMultiplier { get; set; }

        [JsonProperty(PropertyName = "fastJumpMultiplier")]
		public float FastJumpMultiplier { get; set; }

        [JsonProperty(PropertyName = "gameMode")]
		public string GameMode { get; set; }

        [JsonProperty(PropertyName = "customData")]
        public Dictionary<string, object> CustomData { get; set; }

        [JsonProperty(PropertyName = "requiredPCModIDs")]
        public List<string> RequiredPCModIDs { get; set; }

        [JsonProperty(PropertyName = "requiredModIDs")]
        public List<string> RequiredQuestModIDs { get; set; }

        [JsonProperty(PropertyName = "guid")]
        public string GUID { get; set; }

        [JsonProperty(PropertyName = "version")]
        public int Version { get; set; }
    }
}