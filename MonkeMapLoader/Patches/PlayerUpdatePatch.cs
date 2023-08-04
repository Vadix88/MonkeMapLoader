using HarmonyLib;
using GorillaLocomotion;
using UnityEngine;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("LateUpdate", MethodType.Normal)]
    public class PlayerUpdatePatch
    {
        public static int _addedFarPlane = 0;

        public static void Postfix() => Camera.main.farClipPlane += _addedFarPlane;
    }
}
