using System.Threading.Tasks;
using ComputerInterface.Interfaces;
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
            
            Utilla.Events.RoomJoined += OnRoomJoined;

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

            Container.Bind<IComputerModEntry>().To<MyEntry>().AsSingle();

            // I just bind another class here to demonstrate adding a command
            // of course you can request the CommandHandler in any of your types as long as you bind it
            // notice how I use BindInterfacesAndSelfTo
            // since MyModCommandManager inherits the IInitializable interface
            // the class gets instantiated even if no other class needs it
            Container.BindInterfacesAndSelfTo<CompManager>().AsSingle();
        }

        private void OnRoomJoined(bool isPrivate)
        {
            if (!isPrivate)
            {
                //PhotonNetworkController.instance.
                Utilla.Utils.RoomUtils.JoinPrivateLobby();
            }
        }
    }
}