using BepInEx;
using Bepinject;
using System.Collections;
using UnityEngine;
using Utilla;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using VmodMonkeMapLoader.Patches;

namespace VmodMonkeMapLoader
{
    [BepInPlugin("vadix.gorillatag.maploader", "Monke Map Loader", "1.0.0")]
    [BepInDependency("tonimacaroni.computerinterface")]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.1.0")]
    public class MonkeMapLoaderPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            Debug.Log("Monke Map Loader started");
            
            Zenjector.Install<MainInstaller>().OnProject();
        }
    }
}