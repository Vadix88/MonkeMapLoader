using ComputerInterface.Interfaces;
using Photon.Pun;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.ComputerInterface;
using VmodMonkeMapLoader.Models;
using Zenject;

namespace VmodMonkeMapLoader
{
    internal class MainInstaller : Installer
    {
        public override void InstallBindings()
        {
            HarmonyPatches.ApplyHarmonyPatches();
            
            var mapLoaderObject = new GameObject("MapLoader");
            var mapLoader = mapLoaderObject.AddComponent<MapLoader>();
            var mapLoaderGameObject = new MapLoaderObject
            {
                MapLoaderGameObject = mapLoaderObject
            };
            
            Container.Bind<MapLoaderObject>().FromInstance(mapLoaderGameObject).AsSingle();
            Container.Bind<MapLoader>().FromInstance(mapLoader).AsSingle();
            
            //Utilla.Events.RoomJoined += OnRoomJoined;
            
            Container.Bind<IComputerModEntry>().To<MapListEntry>().AsSingle();
            //Container.BindInterfacesAndSelfTo<CommandManager>().AsSingle();
        }

        private void OnRoomJoined(bool isPrivate)
        {
            if (!isPrivate)
            {
                if (PhotonNetwork.InRoom)
                {
                    PhotonNetworkController.instance.attemptingToConnect = false;
                    PhotonNetworkController.instance.currentGorillaParent
                        .GetComponentInChildren<GorillaScoreboardSpawner>().OnLeftRoom();
                    foreach (SkinnedMeshRenderer skinnedMeshRenderer in PhotonNetworkController.instance.offlineVRRig)
                    {
                        if ((UnityEngine.Object)skinnedMeshRenderer != (UnityEngine.Object)null)
                            skinnedMeshRenderer.enabled = true;
                    }

                    PhotonNetwork.Disconnect();
                    PhotonNetwork.ConnectUsingSettings();
                }
                Utilla.Utils.RoomUtils.JoinPrivateLobby();
            }
        }
    }
}