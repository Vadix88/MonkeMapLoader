using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
