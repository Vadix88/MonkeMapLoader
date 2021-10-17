using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using VmodMonkeMapLoader.Behaviours;
using GorillaLocomotion;

namespace VmodMonkeMapLoader.Patches
{
	[HarmonyPatch(typeof(GorillaTagManager))]
	[HarmonyPatch("Awake", MethodType.Normal)]
	internal class PlayerMoveSpeedPatch
	{
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

			Player.Instance.maxJumpSpeed = GorillaTagManager.instance.slowJumpLimit;
			Player.Instance.jumpMultiplier = GorillaTagManager.instance.slowJumpMultiplier;
		}

		public static void SetSpeed(MapDescriptor descriptor)
		{
			_descriptor = descriptor;
			_needToSet = true;
		}
	}
}
