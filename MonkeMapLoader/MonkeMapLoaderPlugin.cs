using BepInEx;
using Bepinject;
using UnityEngine;

namespace VmodMonkeMapLoader
{
    [BepInPlugin("vadix.gorillatag.maploader", "Monke Map Loader", "1.0.0")]
    [BepInDependency("tonimacaroni.computerinterface", "1.4.0")]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.3.0")]
    public class MonkeMapLoaderPlugin : BaseUnityPlugin
    {
        void Awake()
        {
            Helpers.Logger.LogText("-= Monke Map Loader started =-");

            HarmonyPatches.ApplyHarmonyPatches();
            Zenjector.Install<MainInstaller>().OnProject();
        }
    }
}