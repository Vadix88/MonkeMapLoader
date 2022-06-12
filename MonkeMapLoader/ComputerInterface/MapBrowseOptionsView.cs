using ComputerInterface;
using ComputerInterface.ViewLib;
using System.Collections.Generic;
using System.Text;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.ComputerInterface
{
	public class MapBrowseOptionsView : ComputerView
	{
		private readonly UISelectionHandler _selectionHandler;

		private MapBrowseOptions _options;

		Dictionary<int, (int id, string name)> _map = new Dictionary<int, (int, string)>
		{
			{ 1, (3, "DOWNLOADS") },
			{ 2, (5, "VERIFIED DATE") },
			{ 3, (1, "UPLOAD DATE") }, // This is technically the modified date
			{ 4, (0, "MAP NAME") },
			{ 5, (4, "AUTHOR") },
		};

		public MapBrowseOptionsView()
		{
			_selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
			_selectionHandler.MaxIdx = 4;
			_selectionHandler.OnSelected += OnEntrySelected;
			_selectionHandler.ConfigureSelectionIndicator($"<color=#{Constants.Blue}> > </color>", "", "   ", "");
		}

		private void OnEntrySelected(int index)
		{
			if (index == 0)
			{
				_options.IsDescending = !_options.IsDescending;
			}
			else
			{
				_options.OrderBy = _map[index].id;
			}

			DrawView();
		}

		public override async void OnShow(object[] args)
		{
			base.OnShow(args);

			if (args == null || args.Length == 0)
			{
				Text = "No options container provided";
				return;
			}

			_options = args[0] as MapBrowseOptions;

			DrawView();
		}

		void DrawView()
		{
			SetText(str =>
			{
				// str.AppendClr("[^ / v] SELECT            [ENTER] DETAILS", Constants.Blue).AppendLine();
				str.AppendClr("[^ / v] MOVE               [ENTER] SELECT", Constants.Blue).AppendLine();

				str.AppendLine();
				str.AppendLine(" SORT OPTIONS");
				// str.AppendClr(" SORT OPTIONS", Constants.Blue).AppendLine();
				str.AppendLine();

				StringBuilder sortOrder = new StringBuilder();
				if (_options.IsDescending)
				{
					sortOrder.AppendClr("DESCENDING", Constants.Blue);
					sortOrder.Append(" ASCENDING");
				}
				else
				{
					sortOrder.Append("DESCENDING");
					sortOrder.AppendClr(" ASCENDING", Constants.Blue);
				}

				str.AppendLine(_selectionHandler.GetIndicatedText(0, sortOrder.ToString())); ;
				str.AppendLine();

				str.AppendLine(" ORDER BY");
				foreach (var item in _map)
				{
					if (_options.OrderBy == item.Value.id)
					{
						str.BeginColor(Constants.Blue);
					}

					str.AppendLine(_selectionHandler.GetIndicatedText(item.Key, item.Value.name));

					if (_options.OrderBy == item.Value.id)
					{
						str.EndColor();
					}
				}
			});
		}

		public override void OnKeyPressed(EKeyboardKey key)
		{
			if (_selectionHandler.HandleKeypress(key))
			{
				DrawView();
				return;
			}

			if (key == EKeyboardKey.Left || key == EKeyboardKey.Right)
			{
				_selectionHandler.HandleKeypress(EKeyboardKey.Enter);
			}

			switch (key)
			{
				case EKeyboardKey.Back:
					ShowView<MapBrowseView>(true);
					break;
			}
		}
	}
}
