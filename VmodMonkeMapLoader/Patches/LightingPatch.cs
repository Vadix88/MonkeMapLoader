using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(UnityEngine.Experimental.GlobalIllumination.LightDataGI))]
    [HarmonyPatch("Init", MethodType.Normal)]
    class LightingPatch
    {
    }
}
