using UnityEngine;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.Helpers
{
    public class Constants
    {
        public const int MaskLayerHeadTrigger = 15;
        public const int MaskLayerHandTrigger = 18;
        public const int MaskLayerPlayerTrigger = 31;
        public const string DefaultShaderName = "Standard";
        public const string CustomMapsFolderName = "CustomMaps";
        public const string MiscObjectsFolderName = "Misc";
        public const string PluginVersion = "1.2.2";
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
		public const string ForestPath = "Level/forest";
		public const string MonkeMapHubBase = "https://monkemaphub.com/";
		public const string Blue = "8dc2ef";
        public const string Green = "00cc44";
    }
}
