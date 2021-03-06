using GorillaLocomotion;
using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("CollisionsSphereCast", MethodType.Normal)]
    internal class PlayerClimbingPatch
    {
        public static bool disabledBecauseUnclimbable = false;
        internal static void Postfix(Player __instance, ref bool __result, float sphereRadius, RaycastHit hitInfo)
        {
            if (__result)
            {
                if (sphereRadius == __instance.minimumRaycastDistance)
                {
                    // it's a hand, do stuff.
                    SurfaceClimbSettings settings = hitInfo.collider.gameObject.GetComponent<SurfaceClimbSettings>();
                    //if (settings != null && settings.Unclimbable == true) __result = false;
                    if (settings != null && settings.Unclimbable == true)
                    {
                        // UNCLIMBABLE. DAS BAD.
                        if (!__instance.disableMovement)
                        {
                            __instance.disableMovement = true;
                            disabledBecauseUnclimbable = true;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Update", MethodType.Normal)]
    internal class PlayerClimbingMovementResetPatch
    {
        internal static void Postfix(Player __instance)
        {
            if(__instance.disableMovement && PlayerClimbingPatch.disabledBecauseUnclimbable)
            {
                __instance.disableMovement = false;
                PlayerClimbingPatch.disabledBecauseUnclimbable = false;
            }
        }
    }
}
