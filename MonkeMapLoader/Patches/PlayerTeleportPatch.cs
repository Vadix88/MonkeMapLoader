﻿using GorillaLocomotion;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using Logger = VmodMonkeMapLoader.Helpers.Logger;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    internal class PlayerTeleportPatch
    {
        private static bool _isTeleporting = false;
        private static Transform _teleportDestination;

        internal static bool Prefix(Player __instance, ref Vector3 ___lastPosition, ref Vector3[] ___velocityHistory, ref Vector3 ___lastHeadPosition, ref Vector3 ___lastLeftHandPosition, ref Vector3 ___lastRightHandPosition, ref Vector3 ___currentVelocity, ref Vector3 ___denormalizedVelocityAverage)
        {
            if (_isTeleporting)
            {
                var playerRigidBody = __instance.GetComponent<Rigidbody>();
                if (playerRigidBody != null)
                {
                    Vector3 correctedPosition = _teleportDestination.position - __instance.bodyCollider.transform.position + __instance.transform.position;

                    // Throw the player slightly to prevent them from being stuck right away
                    playerRigidBody.velocity = Physics.gravity == new Vector3(0, Helpers.SharedConstants.Gravity, 0) ? Vector3.zero : new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-4.5f, 4.5f));

                    playerRigidBody.isKinematic = true;
                    __instance.transform.position = correctedPosition;
                    Debug.Log(__instance.bodyCollider.transform.position);

                    //__instance.transform.rotation = _teleportDestination.rotation;

                    //__instance.transform.rotation = Quaternion.Euler(__instance.transform.rotation.eulerAngles.x, _teleportDestination.rotation.eulerAngles.y,
                    //    __instance.transform.rotation.eulerAngles.z);
                    __instance.Turn(_teleportDestination.rotation.eulerAngles.y - __instance.headCollider.transform.rotation.eulerAngles.y);

                    ___lastPosition = correctedPosition;
                    ___velocityHistory = new Vector3[__instance.velocityHistorySize];

                    ___lastHeadPosition = __instance.headCollider.transform.position;
                    var leftHandMethod = typeof(Player).GetMethod("GetCurrentLeftHandPosition",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    ___lastLeftHandPosition = (Vector3)leftHandMethod.Invoke(__instance, new object[] { });

                    var rightHandMethod = typeof(Player).GetMethod("GetCurrentRightHandPosition",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    ___lastRightHandPosition = (Vector3)rightHandMethod.Invoke(__instance, new object[] { });
                    ___currentVelocity = Vector3.zero;
                    ___denormalizedVelocityAverage = Vector3.zero;
                    playerRigidBody.isKinematic = false;
                }
                Logger.LogText("Teleported player to: " + _teleportDestination.position);
                _isTeleporting = false;
                return false;
            }

            return true;
        }

        internal static void TeleportPlayer(Transform destination)
        {
            if(_isTeleporting)
                return;

            _teleportDestination = destination;
            _isTeleporting = true;
        }
    }
}
