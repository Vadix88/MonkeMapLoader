using System;
using HarmonyLib;
using UnityEngine;
using GorillaNetworking;
using VmodMonkeMapLoader.Behaviours;
using Logger = VmodMonkeMapLoader.Helpers.Logger;

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
                if (PhotonNetworkController.Instance?.currentGameType != null && MapLoader._lobbyName != null && PhotonNetworkController.Instance.currentGameType.Contains(MapLoader._lobbyName))
                {
                    GorillaTagManager __instance = (GorillaTagManager)GorillaTagManager.instance;

                    object currentlyTag;
                    __instance.currentRoom.CustomProperties.TryGetValue("isCurrentlyTag", out currentlyTag);

                    object gameEnded;
                    __instance.currentRoom.CustomProperties.TryGetValue("timeInfectedGameEnded", out gameEnded);

                    if ((bool)currentlyTag) TriggerRoundEndEvents();
                    if (RoundEndActions._instance != null && gameEnded != null && (double)gameEnded > lastGameEnd)
                    {
                        //it's proper. do whatever with the Room Settings.
                        lastGameEnd = __instance.timeInfectedGameEnded;
                        TriggerRoundEndEvents();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogText("Error when triggering room events:");
                Logger.LogException(e);
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
