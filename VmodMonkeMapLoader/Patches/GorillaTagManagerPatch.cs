using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(GorillaTagManager))]
    [HarmonyPatch("InfectionEnd", MethodType.Normal)]
    internal class GorillaTagManagerPatch
    {
        internal static void Postfix(GorillaTagManager __instance)
        {
            if (PhotonNetworkController.instance.currentGameType.Contains(MapLoader._lobbyName))
            {
                //it's proper. do whatever with the Room Settings.
                if(RoundEndActions._instance != null)
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
    }
}
