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

namespace VmodMonkeMapLoader.Behaviours
{
    [System.Serializable]
    public class Teleporter : MonoBehaviour
    {
        public List<Transform> TeleportPoints;
        public bool TagOnTeleport = false;
        public float TeleportDelay = 0f;

        [HideInInspector]
        public bool JoinGameOnTeleport = false;

        [HideInInspector]
        public bool GoesToTreehouse = false;

        private bool _isTeleporting = false;
        void OnTriggerEnter(Collider collider)
        {
            if (_isTeleporting || TeleportPoints == null || !TeleportPoints.HasAtLeast(0))
                return;

            if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
                return;

            Logger.LogText("Triggered: " + gameObject.name);

            _isTeleporting = true;
            StartCoroutine(TeleportPlayer(TeleportDelay));
        }

        private IEnumerator TeleportPlayer(float time)
        {
            yield return new WaitForSeconds(time);

            if (TeleportPoints == null || !TeleportPoints.HasAtLeast(0))
                yield break;

            var destination = TeleportPoints.Count > 1
                ? TeleportPoints[Random.Range(0, TeleportPoints.Count)]
                : TeleportPoints[0];

            Logger.LogText("Teleporting");

            PlayerTeleportPatch.TeleportPlayer(destination);

            if (TagOnTeleport) TagZone.TagLocalPlayer();
            if (JoinGameOnTeleport) MapLoader.JoinGame();
            if (GoesToTreehouse) MapLoader.ResetMapProperties();

            _isTeleporting = false;
        }
    }
}
