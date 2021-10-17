using System;

namespace VmodMonkeMapLoader.Models
{
    public class MapInfo
    {
        public string FilePath { get; set; }
        public MapPackageInfo PackageInfo { get; set; }

        public string GetLobbyName()
		{
            string lobbyName = PackageInfo.Descriptor.Author + "_" + PackageInfo.Descriptor.Name;
            if(!String.IsNullOrWhiteSpace(PackageInfo.Config.GUID))
            {
                lobbyName = PackageInfo.Config.GUID + "_" + PackageInfo.Config.Version;
            }
            return lobbyName;
		}
    }
}