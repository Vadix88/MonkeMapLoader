using System;
using GorillaLocomotion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using VmodMonkeMapLoader.Patches;
using Logger = VmodMonkeMapLoader.Helpers.Logger;
using Random = UnityEngine.Random;

namespace VmodMonkeMapLoader.Behaviours
{
    public class PlayerTeleporter : MonoBehaviour
    {
        public List<TeleportTarget> Destinations = new List<TeleportTarget>();
        public float TeleportDelay = 0f;

        private bool _isTeleporting = false;
        
        void OnTriggerEnter(Collider collider)
        {
            if (_isTeleporting || !Destinations.Any())
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

            if (Destinations == null || !Destinations.Any())
                yield break;

            var destination = Destinations.Count > 1
                ? Destinations[Random.Range(0, Destinations.Count)]
                : Destinations[0];

            Logger.LogText("Teleporting");

            PlayerTeleportPatch.TeleportPlayer(destination);
            _isTeleporting = false;
        }
    }
}