using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace VmodMonkeMapLoader.Patches
{
    [HarmonyPatch(typeof(PhotonNetworkController))]
    [HarmonyPatch("GetRegionWithLowestPing", MethodType.Normal)]
	internal class ForceRegionPatch
	{
		public static string patchForcedRegion = "";

		internal static void Postfix(ref string __result)
		{
			// if we have a region we want to force and the region is not different, then return that
			if (patchForcedRegion != "" && __result != patchForcedRegion)
			{
				__result = patchForcedRegion;
				patchForcedRegion = "";
			}
		}
	}
}
