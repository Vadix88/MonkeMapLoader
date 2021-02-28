using UnityEngine;

namespace VmodMonkeMapLoader.Helpers
{
    public class Constants
    {
        public const string ObjectNameTeleportTarget = "TeleportTarget_";
        public const string ObjectNameTeleportTrigger = "TeleportTrigger_";
        public const string ObjectNameTreeTeleportTarget = "TreeTeleportTarget";
        public const string ObjectNameTreeTeleportTrigger = "TreeTeleportTrigger";
        public const string ObjectNameTreeTeleportTargetLegacyStart = "Start";
        public const string ObjectNameTreeTeleportTargetLegacyEnd = "End";
        public const int MaskLayerHandTrigger = 18;
        public const string MapRootNameOrigin = "origin";
        public const string MapRootNameLegacy = "map.prefab";
        public static readonly Vector3 LegacyMapOffset = new Vector3(0f, 28f, 0f);
        public const string DefaultShaderName = "Standard";
        public const string CustomMapsFolderName = "CustomMaps";
    }
}