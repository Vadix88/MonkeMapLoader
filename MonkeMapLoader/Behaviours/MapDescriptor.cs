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

#if !PLUGIN
        // Only interact with this in Unity, it isn't in maps. For that, use Events.PackageInfo.Config.CustomData
        public Dictionary<string, object> CustomData = new Dictionary<string, object>();
        public List<string> RequiredPCModsId;
        public List<string> RequiredQuestModsId;
#endif
    }
}
