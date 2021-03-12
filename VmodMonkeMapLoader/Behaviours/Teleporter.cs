using ModestTree;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Logger = VmodMonkeMapLoader.Helpers.Logger;
using System.Collections;
using System.Linq;
using VmodMonkeMapLoader.Patches;
using Random = UnityEngine.Random;
using BepInEx;
using GorillaLocomotion;
using VmodMonkeMapLoader.Helpers;

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class Teleporter : GorillaMapTriggerBase
    {
        public List<Transform> TeleportPoints;
        public bool TagOnTeleport = false;

        [HideInInspector]
        public bool JoinGameOnTeleport = false;

        [HideInInspector]
        public bool GoesToTreehouse = false;

        private bool _isTeleporting = false;

        public override void Trigger(Collider collider)
        {
            if (_isTeleporting || TeleportPoints == null || !TeleportPoints.HasAtLeast(0))
                return;

            _isTeleporting = true;
            StartCoroutine(TeleportPlayer(0f));

            base.Trigger(collider);
        }
        
        private IEnumerator TeleportPlayer(float time)
        {
            foreach(var point in TeleportPoints)
            {
                if (point == null) continue;
                Debug.Log("POINT");
                Debug.Log(point.gameObject.name);
            }
            yield return new WaitForSeconds(time);

            if (TeleportPoints == null || !TeleportPoints.HasAtLeast(0))
                yield break;

            var destination = TeleportPoints.Count > 1
                ? TeleportPoints[Random.Range(0, TeleportPoints.Count)]
                : TeleportPoints[0];

            Logger.LogText("Teleporting player to: " + destination.position);

            PlayerTeleportPatch.TeleportPlayer(destination);

            if (TagOnTeleport) TagZone.TagLocalPlayer();
            if (JoinGameOnTeleport) MapLoader.JoinGame();
            if (GoesToTreehouse) MapLoader.ResetMapProperties();

            _isTeleporting = false;
        }
    }
}
