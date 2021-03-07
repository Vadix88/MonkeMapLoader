using UnityEngine;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.Helpers
{
    public class Constants
    {
        public const int MaskLayerGorillaTrigger = 15;
        public const int MaskLayerHandTrigger = 18;
        public static readonly Vector3 LegacyMapOffset = new Vector3(0f, 28f, 0f);
        public const string DefaultShaderName = "Standard";
        public const string CustomMapsFolderName = "CustomMaps";
        public const string MiscObjectsFolderName = "Misc";
        public static MapInfo MapInfoError = new MapInfo
        {
            PackageInfo = new MapPackageInfo
            {
                Descriptor = new Descriptor
                {
                    Author = "[error]",
                    Name = "[error]",
                    Description = "[error]"
                }
            }
        };
    }
}