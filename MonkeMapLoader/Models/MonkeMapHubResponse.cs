using System;
using System.Collections.Generic;
using System.Text;

namespace VmodMonkeMapLoader.Models
{
	public class MonkeMapHubResponse
	{
		public class MapsRoot
		{
			public bool IsSuccess { get; set; }
			public string ErrorMessage { get; set; }
			public MapsData Data { get; set; }
		}
		public class MapsData
		{
			public List<Map> Maps { get; set; }
			public int Count { get; set; }
		}

		public class Map
		{
			public int MapId { get; set; }
			public string MapGuid { get; set; }
			public int MapVersion { get; set; }
			public string MapName { get; set; }
			public string MapFileUrl { get; set; }
			public string MapThumbnailFileUrl { get; set; }
			public DateTime MapDateAdded { get; set; }
			public DateTime MapDateUpdated { get; set; }
			public int MapDownloadCount { get; set; }
			public string MapFileName { get; set; }
			public int MapFileSize { get; set; }
			public string AuthorDiscordId { get; set; }
			public string AuthorName { get; set; }
			public string AuthorDiscriminator { get; set; }
		}

		public class MapData
		{
			public int MapId { get; set; }
			public string MapGuid { get; set; }
			public int MapVersion { get; set; }
			public string MapName { get; set; }
			public string MapDescription { get; set; }
			public string MapFileUrl { get; set; }
			public string MapThumbnailFileUrl { get; set; }
			public string MapCubemapFileUrl { get; set; }
			public DateTime MapDateAdded { get; set; }
			public DateTime MapDateUpdated { get; set; }
			public int MapDownloadCount { get; set; }
			public bool MapIsVerified { get; set; }
			public string MapFileName { get; set; }
			public int MapFileSize { get; set; }
			public bool MapIsUpdated { get; set; }
			public string AuthorDiscordId { get; set; }
			public string AuthorName { get; set; }
			public string AuthorDiscriminator { get; set; }
			public string AuthorAvatar { get; set; }
		}

		public class MapRoot
		{
			public bool IsSuccess { get; set; }
			public string ErrorMessage { get; set; }
			public MapData Data { get; set; }
		}
	}
}
