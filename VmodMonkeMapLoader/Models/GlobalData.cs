using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VmodMonkeMapLoader.Models
{
    internal class GlobalData
    {
        private Vector3 _origin = Vector3.zero;
        public Vector3 Origin
        {
            get => _origin;
            set
            {
                if (value.Equals(_origin))
                    return;
                _origin = value;
                CustomOrigin = _origin + new Vector3(0f, 54f, 0f);
            }
        }

        public Vector3 CustomOrigin { get; private set; } = new Vector3(0f, 54f, 0f);

        public Dictionary<string, TeleportData> Teleports = new Dictionary<string, TeleportData>();

        private TeleportTarget _teleportTargetToBigTree = new TeleportTarget
        {
            Position = new Vector3(-66f, 12.3f, -83f),
            RotationAngle = -90f
        };

        public Vector3 TreeOrigin = Vector3.zero;

        public TeleportTarget TeleportTargetToBigTree => _teleportTargetToBigTree;

        public GameObject BigTreeTeleportToMap;

        public GameObject TeleportTriggerBackToBigTree;

        public GameObject TeleportPrefab;

        public GameObject FallEmergencyTeleport;
        
        public bool IsLegacyMap = false;
    }
}