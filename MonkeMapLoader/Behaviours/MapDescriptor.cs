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
        public float GravitySpeed = -9.8f;
        public bool ExportLighting = true;

        public float SlowJumpLimit = 6.5f;
        public float FastJumpLimit = 8.5f;
        public float SlowJumpMultiplier = 1.1f;
        public float FastJumpMultiplier = 1.3f;

        public string GameMode = "";
    }
}
