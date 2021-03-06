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

        public Vector3 TreeOrigin = Vector3.zero;

        public GameObject BigTreeTeleportToMap;

        public GameObject BigTreePoint;

        public GameObject FallEmergencyTeleport;

        public bool IsLegacyMap = false;
    }
}