using ModestTree;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

#if PLUGIN
using VmodMonkeMapLoader.Patches;
using Logger = VmodMonkeMapLoader.Helpers.Logger;
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
                TeleportPoints = GameObject.Find("SpawnPointContainer")?.GetComponentsInChildren<Transform>().ToList();
            }

            if (TeleportPoints == null || !TeleportPoints.HasAtLeast(0))
                yield break;

            var destination = TeleportPoints.Count > 1
                ? TeleportPoints[Random.Range(0, TeleportPoints.Count)]
                : TeleportPoints[0];

            Logger.LogText("Teleporting player to: " + destination.position);

            PlayerTeleportPatch.TeleportPlayer(destination);

            if (TagOnTeleport) TagZone.TagLocalPlayer();
            if (JoinGameOnTeleport) MapLoader.JoinGame();
            if (TeleporterType == TeleporterType.Treehouse) MapLoader.ResetMapProperties();

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
