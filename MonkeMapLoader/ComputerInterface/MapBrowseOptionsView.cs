using ComputerInterface;
using ComputerInterface.ViewLib;
using System.Text;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.ComputerInterface
{
	public class MapBrowseOptionsView : ComputerView
	{
		private readonly UISelectionHandler _selectionHandler;

		private MapBrowseOptions _options;

		public MapBrowseOptionsView()
		{
			_selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down, EKeyboardKey.Enter);
			_selectionHandler.MaxIdx = 4;
			_selectionHandler.OnSelected += OnEntrySelected;
			_selectionHandler.ConfigureSelectionIndicator($"<color=#{Constants.Blue}> > </color>", "", "   ", "");
		}

		private void OnEntrySelected(int index)
		{
			switch (index)
			{
				case 0:
					{
						_options.IsDescending = !_options.IsDescending;
						break;
					}
				case 1:
					{
						_options.OrderBy = 0;
						break;
					}
				case 2:
					{
						_options.OrderBy = 1;
						break;
					}
				case 3:
					{
						_options.OrderBy = 3;
						break;
					}
				case 4:
					{
						_options.OrderBy = 4;
						break;
					}
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

			_options =  args[0] as MapBrowseOptions;

			DrawView();
		}

		void DrawView()
		{
			SetText(str =>
			{
				// str.AppendClr("[^ / v] SELECT MAP        [ENTER] DETAILS", Constants.Blue).AppendLine();
				str.AppendClr(" SORT OPTIONS", Constants.Blue).AppendLine();
				str.AppendLine();

				str.AppendLine(_selectionHandler.GetIndicatedText(0, _options.IsDescending ? " DESCENDING" : " ASCENDING"));
				str.AppendLine();

				str.AppendLine(" ORDER BY");
				AppendSelection(str, _selectionHandler, _options.OrderBy == 0, 1, "MAP NAME", Constants.Blue);
				AppendSelection(str, _selectionHandler, _options.OrderBy == 1, 2, "DATE", Constants.Blue);
				AppendSelection(str, _selectionHandler, _options.OrderBy == 3, 3, "DOWNLOADS", Constants.Blue);
				AppendSelection(str, _selectionHandler, _options.OrderBy == 4, 4, "AUTHOR", Constants.Blue);
			});
		}

		void AppendSelection(StringBuilder str, UISelectionHandler selectionHandler, bool condition, int index, string name, string highlightColor)
		{
			if (condition)
			{
				name = new StringBuilder().AppendClr(name, highlightColor).ToString();
			}
			str.AppendLine(_selectionHandler.GetIndicatedText(index, name));
		}

		public override void OnKeyPressed(EKeyboardKey key)
		{
			if (_selectionHandler.HandleKeypress(key))
			{
				DrawView();
				return;
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
