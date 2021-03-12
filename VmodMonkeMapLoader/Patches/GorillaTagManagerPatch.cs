using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(VRRig))]
    [HarmonyPatch("PlayTagSound", MethodType.Normal)]
    internal class GorillaTagManagerPatch
    {
        private static double lastGameEnd = 0;
        internal static void Postfix()
        {
            try
            {
                if (PhotonNetworkController.instance?.currentGameType != null && MapLoader._lobbyName != null && PhotonNetworkController.instance.currentGameType.Contains(MapLoader._lobbyName))
                {
                    GorillaTagManager __instance = GorillaTagManager.instance;
                    if (__instance.isCurrentlyTag) TriggerRoundEndEvents();
                    if (RoundEndActions._instance != null && __instance.timeInfectedGameEnded > lastGameEnd)
                    {
                        //it's proper. do whatever with the Room Settings.
                        lastGameEnd = __instance.timeInfectedGameEnded;
                        TriggerRoundEndEvents();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error when triggering room events:");
                Debug.Log(e);
            }
        }

        static void TriggerRoundEndEvents()
        {
            RoundEndActions roundEndActions = RoundEndActions._instance;
            if (roundEndActions.ObjectsToEnable != null)
            {
                foreach (GameObject objectToEnable in roundEndActions.ObjectsToEnable)
                {
                    if (objectToEnable != null)
                    {
                        objectToEnable.SetActive(false);
                        objectToEnable.SetActive(true);
                    }
                }
            }
            if (roundEndActions.ObjectsToDisable != null)
            {
                foreach (GameObject objectToDisable in roundEndActions.ObjectsToDisable)
                {
                    if (objectToDisable != null)
                    {
                        objectToDisable.SetActive(true);
                        objectToDisable.SetActive(false);
                    }
                }
            }
            if (roundEndActions.RespawnOnRoundEnd) MapLoader.ForceRespawn();
        }
    }
}
