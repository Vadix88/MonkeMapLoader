using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Zenject;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(SceneContextRegistry))]
    [HarmonyPatch("Add", MethodType.Normal)]
    internal class ZenjectPatch
    {
        internal static bool Prefix()
        {
            return false;
        }
    }
}
