using GorillaLocomotion;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update", MethodType.Normal)]
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
                    playerRigidBody.velocity = Vector3.zero;
                    playerRigidBody.isKinematic = true;
                    __instance.transform.position = _teleportDestination.position;
                    //__instance.transform.rotation = _teleportDestination.rotation;

                    __instance.transform.rotation = Quaternion.Euler(__instance.transform.rotation.eulerAngles.x, _teleportDestination.rotation.eulerAngles.y,
                        __instance.transform.rotation.eulerAngles.z);

                    ___lastPosition = _teleportDestination.position;
                    ___velocityHistory = new Vector3[__instance.velocityHistorySize];

                    ___lastHeadPosition = __instance.headCollider.transform.position;
                    var leftHandMethod = typeof(Player).GetMethod("CurrentLeftHandPosition",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    ___lastLeftHandPosition = (Vector3)leftHandMethod.Invoke(__instance, new object[] { });
                    var rightHandMethod = typeof(Player).GetMethod("CurrentRightHandPosition",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    ___lastRightHandPosition = (Vector3)rightHandMethod.Invoke(__instance, new object[] { });
                    ___currentVelocity = Vector3.zero;
                    ___denormalizedVelocityAverage = Vector3.zero;
                    playerRigidBody.isKinematic = false;
                }
                Debug.Log("_______====== Tepppp");
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