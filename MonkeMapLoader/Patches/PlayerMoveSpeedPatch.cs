using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using VmodMonkeMapLoader.Behaviours;

namespace VmodMonkeMapLoader.Patches
{
	[HarmonyPatch(typeof(GorillaTagManager))]
	[HarmonyPatch("Awake", MethodType.Normal)]
	internal class PlayerMoveSpeedPatch
	{
		// If a player goes into a teleport, leaves the map, and hits disconnect
		// before they connect to a lobby, this could accidnetally set the wrong value.
		// Oh well!

		static bool _needToSet = false;
		static MapDescriptor _descriptor;

		internal static void Postfix(GorillaTagManager __instance)
		{
			if (_needToSet)
			{
				_needToSet = false;
				GorillaTagManager.instance.slowJumpLimit = _descriptor.SlowJumpLimit;
				GorillaTagManager.instance.slowJumpMultiplier = _descriptor.SlowJumpMultiplier;
				GorillaTagManager.instance.fastJumpLimit = _descriptor.FastJumpLimit;
				GorillaTagManager.instance.fastJumpMultiplier = _descriptor.FastJumpMultiplier;
			}
		}

		public static void SetSpeed(MapDescriptor descriptor)
		{
			_descriptor = descriptor;
			_needToSet = true;
		}
	}
}
