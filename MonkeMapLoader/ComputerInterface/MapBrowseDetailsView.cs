using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ComputerInterface;
using ComputerInterface.ViewLib;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using System.Net.Http;
using Newtonsoft.Json;
using static VmodMonkeMapLoader.Models.MonkeMapHubResponse;

namespace VmodMonkeMapLoader.ComputerInterface
{
	public class MapBrowseDetailsView : ComputerView
	{
		private readonly MapLoader _mapLoader;
		private readonly HttpClient _client;

		private Map _map;
		private bool _isError;

		private CancellationTokenSource _loaderCancelToken;

		// TODO: Check if there is an update
		private bool _isDownloaded;
		private Models.MapInfo mapInfo;

		private MapData _mapData;

		public MapBrowseDetailsView(MapLoader mapLoader, HttpClient client)
		{
			_mapLoader = mapLoader;
			_client = client;
		}

		public override async void OnShow(object[] args)
		{
			base.OnShow(args);

			MapLoader.HideTreeTeleporter();

			if (args == null || args.Length == 0)
			{
				if (_map == null)
				{
					Text = "No map selected;";
					_isError = true;
					return;
				}
			}
			else
			{
				_map = args[0] as Map;
			}

			_isError = false;

			_isDownloaded = _mapLoader.IsMapDownloaded(_map, out mapInfo);

			string requestUri = $"{Constants.MonkeMapHubBase}api/maps/{_map.MapGuid}";
			var response = await _client.GetAsync(requestUri);

			string obj = await response.Content.ReadAsStringAsync();
			_mapData = JsonConvert.DeserializeObject<MapRoot>(obj).Data;

			PrintMapInfo();
		}

		public override void OnKeyPressed(EKeyboardKey key)
		{
			if (_isError)
			{
				_isError = false;
				ShowView<MapBrowseView>();
				return;
			}

			switch (key)
			{
				case EKeyboardKey.Back:
					_map = null;
					ShowView<MapBrowseView>();
					break;

				case EKeyboardKey.Enter:
					if (_isDownloaded)
					{
						MapLoader.ShowTreeTeleporter();
						ShowView<MapDetailsView>(mapInfo, typeof(MapBrowseDetailsView));
					}
					Text = "Downloading map: " + _map.MapName;
					_mapLoader.DownloadMap(_map, OnMapDownloaded);
					break;
			}
		}

		private void PrintMapInfo()
		{
			// TODO: Have enter load the map if downloaded
			var sb = new StringBuilder();

			if (_isDownloaded)
			{
				sb.AppendClr("<noparse> << [BACK]          [ENTER]      LOAD MAP</noparse>", Constants.Blue).AppendLine();
			}
			else
			{
				sb.AppendClr("<noparse> << [BACK]          [ENTER]  DOWNLOAD MAP</noparse>", Constants.Blue).AppendLine();
			}

			sb.AppendLines(2);

			int nameWidth = _isDownloaded ? SCREEN_WIDTH - 13 : SCREEN_WIDTH;

			sb.AppendClr(_map.MapName.Clamp(nameWidth).PadRight(nameWidth), Constants.Green);
			if (_isDownloaded)
			{
				sb.AppendLine(" [DOWNLOADED]");
			}
			else
			{
				sb.AppendLine();
			}

			sb.Append("BY: ").AppendClr(_map.AuthorName, Constants.Green).AppendLine($" ({_map.AuthorName}#{_map.AuthorDiscriminator})");

			sb.AppendClr(NumberFormatUtils.FormatCount(_map.MapDownloadCount), Constants.Green).Append(" DOWNLOADS")
				.Append($"  {NumberFormatUtils.FormatSize(_map.MapFileSize, Constants.Green)}")
				.AppendLine($"  {_map.MapDateUpdated:yyyy-mm-dd}");

			sb.AppendLine();
			sb.AppendLine(_mapData.MapDescription);

			Text = sb.ToString();
		}

		private async void OnMapDownloaded(bool success)
		{
			SetText(str =>
			{
				str.BeginCenter().Append(success ? "Map Downloaded!" : "Error Downloading Map").EndAlign();
			});

			if (success)
			{
				_isDownloaded = _mapLoader.IsMapDownloaded(_map, out mapInfo, true);
			}

			_loaderCancelToken?.Cancel();
			_loaderCancelToken?.Dispose();
			_loaderCancelToken = new CancellationTokenSource();

			await HideMapDownloaderText(_loaderCancelToken.Token);
		}

		private async Task HideMapDownloaderText(CancellationToken token)
		{
			try
			{
				await Task.Delay(3000, token);

				PrintMapInfo();
			}
			catch (OperationCanceledException) { }
		}
	}
}