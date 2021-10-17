using System;
using System.Collections.Generic;
using System.Text;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader
{
	public class Events
	{
		// This doesn't really need to be here, because Descriptor is also here
		public static string MapName
		{
			get
			{
				return MapLoader._descriptor != null ? MapLoader._descriptor.MapName : null;
			}
		}

		public static string MapFileName
		{
			get
			{
				return Behaviours.MapLoader._mapFileName;
			}
		}

		public static MapDescriptor Descriptor
		{
			get
			{
				return MapLoader._descriptor;
			}
		}

		public static MapPackageInfo PackageInfo
		{
			get
			{
				return MapLoader._mapInfo.PackageInfo;
			}
		}

		public static Action<bool> OnMapChange { get; set; }

		public static Action<bool> OnMapEnter { get; set; }
	}
}
