using UnityEngine;
using System.Collections.Generic;

namespace VmodMonkeMapLoader.Behaviours
{
    public class MapDescriptor : MonoBehaviour
    {
        public string MapName = "Map";
        public string AuthorName = "Author";
        public string Description = string.Empty;
        public Transform[] SpawnPoints;
        public Cubemap CustomSkybox;
        public float GravitySpeed = Helpers.SharedConstants.Gravity;
        public bool ExportLighting = true;

        public float SlowJumpLimit = Helpers.SharedConstants.SlowJumpLimit;
        public float FastJumpLimit = Helpers.SharedConstants.FastJumpLimit;
        public float SlowJumpMultiplier = Helpers.SharedConstants.SlowJumpMultiplier;
        public float FastJumpMultiplier = Helpers.SharedConstants.FastJumpMultiplier;

        public string GameMode = "";

        public Dictionary<string, string> CustomData = new Dictionary<string, string>();
        public List<string> CustomDataKeys = new List<string>();
        public List<string> CustomDataValues = new List<string>();

        void Awake()
        {
            if (CustomDataKeys.Count != CustomDataValues.Count) return;
            CustomData = new Dictionary<string, string>();
            for (int i = 0; i < CustomDataKeys.Count; i++)
            {
                CustomData.Add(CustomDataKeys[i], CustomDataValues[i]);
            }
        }
    }
}
