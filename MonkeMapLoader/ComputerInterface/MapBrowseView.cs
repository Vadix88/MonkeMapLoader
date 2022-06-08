using ComputerInterface;
using ComputerInterface.ViewLib;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using static VmodMonkeMapLoader.Models.MonkeMapHubResponse;

namespace VmodMonkeMapLoader.ComputerInterface
{
	public class MapBrowseView : ComputerView
	{
		private const int PageSize = 9;
		private readonly UIElementPageHandler<Map> _pageHandler;
		private readonly UISelectionHandler _selectionHandler;
		private readonly HttpClient _client;

		private static List<Map> _mapList = new List<Map>();
		private Map _selectedMap;

		private bool _isError;

		private MapsRoot _mapResponse;

		private MapBrowseView(HttpClient client)
		{
			_selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
			_selectionHandler.OnSelected += OnMapSelected;

			_pageHandler = new UIElementPageHandler<Map>(EKeyboardKey.Left, EKeyboardKey.Right);
			_pageHandler.Footer = "    {2}:{3}  {0} {1}";
			_pageHandler.EntriesPerPage = PageSize;

			_client = client;
		}

		public override async void OnShow(object[] args)
		{
			base.OnShow(args);

			if (_mapResponse == null)
			{
				SetText(str =>
				{
					str.Repeat("=", SCREEN_WIDTH).AppendLine();
					str.BeginCenter().Append("Monke Map Hub").AppendLine();
					str.Append("by ").AppendClr("Vadix", "3fbc04").Append(" & ").AppendClr("Bobbie", Constants.Blue).EndAlign().AppendLine();
					str.Repeat("=", SCREEN_WIDTH);
				});

				// TODO: Make all the pages
				_mapResponse = await GetMapsAsync(PageSize, 0);
			}

			RefreshMapList();
		}

		private async Task<MapsRoot> GetMapsAsync(int pageSize, int pageNumber)
		{
			var body = new
			{
				paginationInfo = new
				{
					pageSize = pageSize,
					pageNumber = pageNumber,
					orderBy = 3,
					isDescending = true
				}
			};

			var bodyContent = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

			var response = await _client.PostAsync($"{Constants.MonkeMapHubBase}api/maps", bodyContent);
			return JsonConvert.DeserializeObject<MapsRoot>(await response.Content.ReadAsStringAsync());
		}

		private void RefreshMapList()
		{
			if (_mapList == null || _mapList.Count == 0)
			{
				_mapList = _mapResponse.Data.Maps;
				_mapList.Add(null);
				_pageHandler.SetElements(_mapList.ToArray());
				_selectionHandler.CurrentSelectionIndex = 0;
			}

			_isError = false;
			DrawList();

			var selectedIdx = _pageHandler.GetAbsoluteIndex(_selectionHandler.CurrentSelectionIndex);

			if (_mapList.Count > 0)
				PreviewOrb.ChangeOrb(_mapList[selectedIdx]);
		}

		private void DrawList()
		{
			if (_mapList.Count == 0)
			{
				SetText(DrawNoMaps);
				_isError = true;
				return;
			}

			if (_isError)
			{
				return;
			}

			var str = new StringBuilder();
			str.AppendClr("[^ / v] SELECT MAP        [ENTER] DETAILS", Constants.Blue).AppendLine();
			//str.AppendClr("[OPT 1] OPTIONS".PadLeft(SCREEN_WIDTH), Constants.Blue).AppendLine();
			str.AppendLine();

			_selectionHandler.MaxIdx = _pageHandler.ItemsOnScreen - 1;

			_pageHandler.EnumarateElements((map, idx) =>
			{
				if (map == null)
				{
					str.AppendLine();
					return;
				}

				var isSelected = idx == _selectionHandler.CurrentSelectionIndex;

				if (isSelected) str.BeginColor(Constants.Blue).Append("> ");
				else str.Append("  ");

				str.Append(map.MapName.Clamp(32).PadRight(32));

				if (isSelected) str.EndColor();

				int downloadCount = map.MapDownloadCount;
				str.Append(NumberFormatUtils.FormatCount(downloadCount));

				str.AppendLine();
			});

			str.AppendLine();

			_pageHandler.AppendFooter(str);

			SetText(str);

			var selectedIdx = _pageHandler.GetAbsoluteIndex(_selectionHandler.CurrentSelectionIndex);
			if (_mapList[selectedIdx] != null)
			{
				_selectedMap = _mapList[selectedIdx];
				PreviewOrb.ChangeOrb(_selectedMap);
			}
		}

		private void DrawNoMaps(StringBuilder str)
		{
			// TODO: Update
			str.Append("NO CUSTOM MAPS FOUND.").Repeat("\n", 2);
			str.Append("If you have map files in the folder").AppendLine();
			str.Append(" make sure they are in the right format.").Repeat("\n", 2);
			str.Append("You can find maps on the website:").AppendLine();
			str.BeginCenter().AppendClr("MonkeMapHub.com", Constants.Blue).EndAlign().Repeat("\n", 2);
			str.BeginCenter().Append("PRESS ANY BUTTON TO CONTINUE...").EndAlign();
		}

		private void OnMapSelected(int _)
		{
			if (_selectedMap == null) return;

			ShowView<MapBrowseDetailsView>(_selectedMap);
		}

		public override async void OnKeyPressed(EKeyboardKey key)
		{
			if (_isError)
			{
				_isError = false;
				ReturnToMainMenu();
				return;
			}

			if (_selectionHandler.HandleKeypress(key))
			{
				var selectedIdx = _pageHandler.GetAbsoluteIndex(_selectionHandler.CurrentSelectionIndex);
				if (_mapList[selectedIdx] == null) _selectionHandler.MoveSelectionUp();
				DrawList();
				return;
			}

			if (_pageHandler.HandleKeyPress(key))
			{
				_selectionHandler.CurrentSelectionIndex = 0;

				if (_pageHandler.CurrentPage == _pageHandler.MaxPage)
				{
					if (_mapResponse.Data.Count > _mapList.Count)
					{
						int currentPage = _pageHandler.CurrentPage;
						_mapResponse = await GetMapsAsync(PageSize, _pageHandler.CurrentPage);
						_mapList.RemoveAt(_mapList.Count - 1);
						_mapList.AddRange(_mapResponse.Data.Maps);
						_mapList.Add(null);
						_pageHandler.SetElements(_mapList.ToArray());
						_pageHandler.CurrentPage = currentPage;
					}
				}

				DrawList();
				return;
			}

			switch (key)
			{
				case EKeyboardKey.Back:
					PreviewOrb.HideOrb();
					ReturnToMainMenu();
					break;
			}
		}
	}
}