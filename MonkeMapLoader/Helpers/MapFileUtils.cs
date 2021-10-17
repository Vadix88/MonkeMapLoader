using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.Helpers
{
    public static class MapFileUtils
    {
        public static List<MapInfo> GetMapList()
        {
            try
            {
                var dirPath = Path.Combine(Path.GetDirectoryName(typeof(MapFileUtils).Assembly.Location), Constants.CustomMapsFolderName);
                Logger.LogText("Map folder: " + dirPath);
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var files = Directory.GetFiles(dirPath, "*.gtmap", SearchOption.TopDirectoryOnly);
                // Don't log this for perf reasons
                //Logger.LogText("Files found: " + string.Join(" | ", files));
                var mapPackagesInfo = files
                    .Select(f => new MapInfo
                    {
                        FilePath = f,
                        PackageInfo = GetMapInfoFromZip(f)
                    })
                    .Where(pi => pi.PackageInfo != null)
                    .ToList();

                Logger.LogText("Maps found: " + string.Join(" | " , mapPackagesInfo.Select(mp => mp.PackageInfo.Descriptor.Name).ToList()));

                return mapPackagesInfo;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);

                return new List<MapInfo>();
            }
        }

        public static MemoryStream GetMapDataStreamFromZip(MapInfo mapInfo)
        {
            using (var archive = ZipFile.OpenRead(mapInfo.FilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.Name != mapInfo.PackageInfo.PcFileName)
                        continue;

                    var seekableStream = new MemoryStream();
                    entry.Open().CopyTo(seekableStream);
                    seekableStream.Position = 0;
                    return seekableStream;
                } 
            }

            return null;
        }

        private static MapPackageInfo GetMapInfoFromZip(string path)
        {
            MapPackageInfo mapPackageInfo;
            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                var jsonEntry = archive.Entries.FirstOrDefault(i => i.Name == "package.json");
                if (jsonEntry == null)
                    return null;

                using (var stream = new StreamReader(jsonEntry.Open(), Encoding.Default))
                {
                    string jsonString = stream.ReadToEnd();
                    mapPackageInfo = JsonConvert.DeserializeObject<MapPackageInfo>(jsonString);

                    // Cubemap Stuff
                    if (mapPackageInfo.Config.CubemapImagePath != null)
                    {
                        var cubemapEntry = archive.Entries.FirstOrDefault(i => i.Name == mapPackageInfo.Config.CubemapImagePath);
                        if (cubemapEntry != null)
                        {
                            var seekableStream = new MemoryStream();
                            cubemapEntry.Open().CopyTo(seekableStream);
                            seekableStream.Position = 0;
                            mapPackageInfo.PreviewCubemap = Texture2DFromStream(seekableStream);
                        }
                    }
                }
            }
            return mapPackageInfo;
        }

        public static AssetBundle GetAssetBundleFromZip(string path)
        {
            MapInfo mapInfo = new MapInfo();
            mapInfo.FilePath = path;
            mapInfo.PackageInfo = GetMapInfoFromZip(path);
            return AssetBundle.LoadFromStream(GetMapDataStreamFromZip(mapInfo));
        }

        public static Texture2D Texture2DFromStream(MemoryStream stream)
        {

            Texture2D tex = new Texture2D(2, 2);
            ImageConversion.LoadImage(tex, stream.ToArray());
            return tex;
        }
    }
}