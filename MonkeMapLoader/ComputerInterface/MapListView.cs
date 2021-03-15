using ComputerInterface;
using ComputerInterface.ViewLib;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MapListView : ComputerView
    {
        private List<MapInfo> _mapList = new List<MapInfo>();
        private bool _isFirstView = true;
        private int _mapSelection;
        private bool _isError;
        private int _currentPage = 1;
        private const int _pageSize = 9;
        private int _mapCount;
        private int _totalPages;

        private readonly UISelectionHandler _selectionHandler;

        private MapListView()
        {
            _selectionHandler = new UISelectionHandler(EKeyboardKey.Up, EKeyboardKey.Down);
        }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            
            if (_isFirstView)
            {
                var str = new StringBuilder();
                str.Repeat("=", SCREEN_WIDTH).AppendLine();
                str.BeginCenter().Append("Monke Map Loader").AppendLine();
                str.Append("by ").AppendClr("Vadix", "3fbc04").Append(" & ").AppendClr("Bobbie", "8dc2ef").EndAlign().AppendLine();
                str.Repeat("=", SCREEN_WIDTH);
                Text = str.ToString();
            }
            else
            {
                RefreshMapList();
            }
        }
        
        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (_isFirstView)
            {
                _isFirstView = false;
                RefreshMapList();
                return;
            }

            if (_isError)
            {
                _isError = false;
                ReturnToMainMenu();
                return;
            }

            if (_selectionHandler.HandleKeypress(key))
            {
                DrawList();
                return;
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    ReturnToMainMenu();
                    break;

                case EKeyboardKey.Enter:
                    if(_mapCount == 0)
                        break;
                    ShowView<MapDetailsView>(_mapList[_mapSelection]);
                    return;

                case EKeyboardKey.Left:
                    if (_currentPage > 1)
                    {
                        _selectionHandler.CurrentSelectionIndex = 0;
                        _currentPage--;
                    }
                    break;

                case EKeyboardKey.Right:
                    if (_currentPage < _totalPages)
                    {
                        _selectionHandler.CurrentSelectionIndex = 0;
                        _currentPage++;
                    }
                    break;
            }

            DrawList();
        }

        private void RefreshMapList()
        {
            _mapList = MapFileUtils.GetMapList();
            _mapSelection = 0;
            _currentPage = 1;
            _mapCount = _mapList.Count;
            _totalPages = (int)Math.Ceiling((decimal)_mapCount / (decimal)_pageSize);
            _isError = false;
            DrawList();
            PreviewOrb.ChangeOrb(_mapList[0]);
        }

        private void DrawList()
        {
            var str = new StringBuilder();

            if (_mapCount == 0)
            {
                str.Append("NO CUSTOM MAPS FOUND.").Repeat("\n", 2);
                str.Append("If you have map files in the folder").AppendLine();
                str.Append(" make sure they are in the right format.").Repeat("\n", 2);
                str.Append("PRESS ANY BUTTON TO CONTINUE...");
                Text = str.ToString();
                _isError = true;
                return;
            }

            if (_isError)
            {
                return;
            }
            
            str.AppendClr("[^ / v] SELECT MAP          [ENTER]  DETAILS", "8dc2ef").AppendLine();
            
            var startIndex = (_currentPage - 1) * _pageSize;
            var endIndex = Math.Min(startIndex + _pageSize - 1, _mapCount - 1);
            _selectionHandler.Max = endIndex - startIndex;
            var selectedIdx = _selectionHandler.CurrentSelectionIndex;
            var line = 0;
            for (var i = startIndex; i <= endIndex; i++)
            {
                var mapName = _mapList[i].PackageInfo.Descriptor.Name.Length > 31
                    ? _mapList[i].PackageInfo.Descriptor.Name.Substring(0, 30) + ".."
                    : _mapList[i].PackageInfo.Descriptor.Name;

                str.Append(line == selectedIdx ? "<color=#00cc44>> " : "   ");
                str.Append(mapName);
                if (line == selectedIdx) str.Append("</color>");
                if(i!=endIndex) str.AppendLine();

                line++;
                _mapSelection = startIndex + selectedIdx;
            }

            str.Repeat("\n", _pageSize - (_selectionHandler.Max + 1));


            str.Append(_currentPage > 1 ? "<noparse><<      </noparse>" : "         ");
            str.Append($"<noparse>{_currentPage,3} : {_totalPages,-4}</noparse>");
            str.Append(_currentPage < _totalPages ? "<noparse>      >></noparse>" : "        ");
            //"  [ ^/v - selection   </> - change page ]"   {i + 1,2}.
            Text = str.ToString();
            int selectedMap = ((_currentPage - 1) * _pageSize) + selectedIdx;
            PreviewOrb.ChangeOrb(_mapList[selectedMap]);
        }
    }
}