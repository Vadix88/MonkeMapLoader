using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using ComputerInterface;
using ComputerInterface.ViewLib;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using Zenject;
using System.Net.Http;
using static VmodMonkeMapLoader.Models.MonkeMapHubResponse;
using Newtonsoft.Json;

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

        private MapData _mapData;

        public MapBrowseDetailsView(MapLoader mapLoader, HttpClient client)
        {
            _mapLoader = mapLoader;
            _client = client;
        }

        public override async void OnShow(object[] args)
        {
            base.OnShow(args);

            if (args == null || args.Length == 0)
            {
                Text = "No map selected;";
                _isError = true;
                return;
            }

            var mapInfo = args[0] as Map;

            _isError = false;
            _map = mapInfo;

			_isDownloaded = _mapLoader.IsMapDownloaded(_map);

            Debug.Log("MAP GUID: " + _map.MapGuid);
			string requestUri = $"{Constants.MonkeMapHubBase}api/maps/{_map.MapGuid}";
			Debug.Log("MAP URI: " + requestUri);
			var response = await _client.GetAsync(requestUri);

			string obj = await response.Content.ReadAsStringAsync();
            Debug.Log(obj);
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
                        break;
                    Text = "Downloading map: " + _map.MapName;
                    _mapLoader.DownloadMap(_map, OnMapDownloaded);
                    break;
            }
        }

        private void PrintMapInfo()
		{
            var sb = new StringBuilder()
                .AppendClr("<noparse> << [BACK]          [ENTER]  DOWNLOAD MAP</noparse>", Constants.Blue).AppendLine()
                .AppendLine()
                .AppendLine();

            int nameWidth = SCREEN_WIDTH;
			
			if (_isDownloaded)
			{
                nameWidth = SCREEN_WIDTH - 13;
			}

            sb.AppendClr(_map.MapName.Clamp(nameWidth).PadRight(nameWidth), Constants.Green);
			if (_isDownloaded)
			{
				sb.AppendLine(" [DOWNLOADED]");
			} else
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

            _loaderCancelToken?.Cancel();
            _loaderCancelToken?.Dispose();
            _loaderCancelToken = new CancellationTokenSource();

            await HideMapDownloaderText(_loaderCancelToken.Token);
        }

        private async Task HideMapDownloaderText(CancellationToken token)
        {
            try
            {
                await Task.Delay(5000, token);

				PrintMapInfo();
            }
            catch (OperationCanceledException) { }
        }
    }
}