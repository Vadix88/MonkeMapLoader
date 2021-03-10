using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using VmodMonkeMapLoader.Behaviours;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(PhotonNetworkController))]
    [HarmonyPatch("OnJoinedRoom", MethodType.Normal)]
    internal class PhotonNetworkControllerPatch
    {
        private static void Prefix(PhotonNetworkController __instance)
        {
            if(__instance.currentGameType != null)
            {
                if(MapLoader._lobbyName != null)
                {
                    if (__instance.currentGameType.Contains(MapLoader._lobbyName) == false) MapLoader.ResetMapProperties();
                }
            }
            else
            {
                MapLoader.ResetMapProperties();
            }
        }
    }
}