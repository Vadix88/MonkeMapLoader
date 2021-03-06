using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class RoundEndActions : MonoBehaviour
    {
        public bool RespawnOnRoundEnd = false;
        public GameObject[] ObjectsToEnable;
        public GameObject[] ObjectsToDisable;
        [HideInInspector]
        [System.NonSerialized]
        public static RoundEndActions _instance; // I'm sorry auros

        void Start()
        {
            _instance = this;
        }
    }
}
