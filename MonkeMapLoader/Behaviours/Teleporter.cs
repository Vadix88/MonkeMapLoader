using ModestTree;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

#if PLUGIN
using VmodMonkeMapLoader.Patches;
#endif

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class Teleporter : GorillaMapTriggerBase
    {
        public List<Transform> TeleportPoints;
        public bool TagOnTeleport = false;

#if PLUGIN

        [HideInInspector]
        public bool JoinGameOnTeleport = false;

        [HideInInspector]
        public TeleporterType TeleporterType = TeleporterType.Normal;

        private bool _isTeleporting = false;

        public override void Trigger(Collider collider)
        {
            if (_isTeleporting || TeleportPoints == null || !TeleportPoints.HasAtLeast(0))
                return;

            _isTeleporting = true;
            StartCoroutine(TeleportPlayer());

            base.Trigger(collider);
        }
        
        private IEnumerator TeleportPlayer()
        {
            if (TeleporterType == TeleporterType.Map) 
            {
                TeleportPoints = GameObject.Find("SpawnPointContainer")?.GetComponentsInChildren<Transform>().Where(e => e != null && e.gameObject.name != "SpawnPointContainer").ToList();
            }

            if (TeleportPoints == null || !TeleportPoints.HasAtLeast(0))
            {
                if (TeleporterType == TeleporterType.Map) TeleportPoints = new List<Transform>() { GameObject.Find("TreeHomeTargetObject").transform };
                else yield break;
            }

            var destination = TeleportPoints.Count > 1
                ? TeleportPoints[Random.Range(0, TeleportPoints.Count)]
                : TeleportPoints[0];
            
            if (TagOnTeleport) TagZone.TagLocalPlayer();
            if (JoinGameOnTeleport) MapLoader.JoinGame();
            if (TeleporterType == TeleporterType.Treehouse) MapLoader.ResetMapProperties();

            PlayerTeleportPatch.TeleportPlayer(destination);

            _isTeleporting = false;
        }

#endif

    }

    public enum TeleporterType
    {
        Normal,
        Treehouse,
        Map
    }
}
