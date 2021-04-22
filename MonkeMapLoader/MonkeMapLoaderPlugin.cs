using BepInEx;
using Bepinject;
using UnityEngine;

namespace VmodMonkeMapLoader
{
    [BepInPlugin("vadix.gorillatag.maploader", "Monke Map Loader", Helpers.Constants.PluginVersion)]
    [BepInDependency("tonimacaroni.computerinterface", "1.4.0")]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.3.2")]
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