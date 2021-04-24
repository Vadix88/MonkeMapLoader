using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;

namespace VmodMonkeMapLoader.Patches
{
	[HarmonyPatch(typeof(Player))]
	[HarmonyPatch("GetSlidePercentage", MethodType.Normal)]
	internal class PlayerClimbingPatch
	{
		internal static bool Prefix(Player __instance, ref float __result, RaycastHit raycastHit)
		{
			// Check for surface component
			Surface surface = raycastHit.collider.GetComponent<Surface>();
			if (surface != null)
			{
				__result = surface.slipPercentage;
				SurfaceClimbSettings surfaceClimbSettings = raycastHit.collider.GetComponent<SurfaceClimbSettings>();
				if (surfaceClimbSettings != null && surfaceClimbSettings.Unclimbable == true) __result = 1;
				return false;
			}

			// Check for unreadable mesh
			MeshCollider meshCollider = raycastHit.collider as MeshCollider;
			if (meshCollider == null || meshCollider.sharedMesh == null)
			{
				__result = __instance.defaultSlideFactor;
				return false;
			}
			if (!meshCollider.sharedMesh.isReadable)
			{
				__result = __instance.defaultSlideFactor;
				return false;
			}

			return true;
		}
	}
}
