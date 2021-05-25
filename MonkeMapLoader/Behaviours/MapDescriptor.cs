using UnityEngine;

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
    }
}
