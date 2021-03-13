using System.Collections;
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
            Container.Bind<SharedCoroutineStarter>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesAndSelfTo<MapLoader>().AsSingle();

            Container.Bind<IComputerModEntry>().To<MapListEntry>().AsSingle();
        }
    }
}