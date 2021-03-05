using System.Threading.Tasks;
using ComputerInterface.Interfaces;
using Photon.Pun;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.ComputerInterface;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using Zenject;
using Logger = VmodMonkeMapLoader.Helpers.Logger;

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

            //Task.Factory.StartNew(() =>
            //{
            //    Task.Delay(5000).Wait();
            //    Utilla.Utils.RoomUtils.JoinPrivateLobby();
            //});

            //Task.Factory.StartNew(() =>
            //{
            //    Task.Delay(5000).Wait();
            //    mapLoader.LoadMap(new MapInfo
            //    {
            //        FilePath =
            //            @"C:\gry\Steam\steamapps\common\Gorilla Tag\BepInEx\plugins\VmodMonkeMapLoader\CustomMaps\testmap6.zip",
            //        PackageInfo = new MapPackageInfo
            //        {
            //            PcFileName = @"maptest6",
            //            Descriptor = new MapDescriptor
            //            {
            //                Name = "Test map 6"
            //            },
            //            Config = new MapConfig
            //            {
            //                RootObjectName = "Origin"
            //            }
            //        }
            //    }, b => Logger.LogText("_____SUCCESS!!!!!!!!"));
            //});

            Container.Bind<IComputerModEntry>().To<MapListEntry>().AsSingle();
            Container.BindInterfacesAndSelfTo<CommandManager>().AsSingle();
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