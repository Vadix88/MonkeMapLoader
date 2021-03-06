using ComputerInterface;
using ComputerInterface.ViewLib;
using System;
using System.Collections.Generic;
using System.Text;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using Zenject;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MapListView : ComputerView
    {
        [Inject]
        public MapLoader MapLoader { get; set; }

        private List<MapInfo> _mapList = new List<MapInfo>();
        private bool _isFirstView = true;
        private int _mapSelection = 0;
        private int _selectedRow = 0;
        private bool _isError = false;
        private int _currentPage = 1;
        private int _maxRowOnPage = 0;
        private const int _pageSize = 9;
        private int _mapCount = 0;
        private int _totalPages = 0;
        private bool _mapListLoaded = false;

        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            
            if (_isFirstView)
            {
                Text = "========================================\n"
                +      "                              Monke Map Loader\n"
                + "                           by <color=#3fbc04>Vadix</color> & <color=#8dc2ef>Bobbie</color>\n"
                +      "========================================";
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
                ReturnView();
                return;
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    ReturnView();
                    break;

                case EKeyboardKey.Enter:
                    Text = "Loaded: " + _mapList[_mapSelection].PackageInfo.Descriptor.Name;
                    _isError = true;
                    MapLoader.LoadMap(_mapList[_mapSelection], b => 
                    //MapLoader.LoadMap(new MapInfo
                    //{
                    //    FilePath =
                    //        @"C:\gry\Steam\steamapps\common\Gorilla Tag\BepInEx\plugins\VmodMonkeMapLoader\CustomMaps\testmap6.zip",
                    //    PackageInfo = new MapPackageInfo
                    //    {
                    //        PcFileName = @"maptest6",
                    //        Descriptor = new MapDescriptor
                    //        {
                    //            Name = "Test map 6"
                    //        },
                    //        Config = new MapConfig
                    //        {
                    //            RootObjectName = "origin"
                    //        }
                    //    }
                    //}, b =>
                    Text = "SUCCESS!!!!!!!!");
                    break;

                case EKeyboardKey.Left:
                    if (_currentPage > 1)
                        _currentPage--;
                    break;

                case EKeyboardKey.Right:
                    if (_currentPage < _totalPages)
                        _currentPage++;
                    break;

                case EKeyboardKey.Up:
                    if (_selectedRow > 0)
                        _selectedRow--;
                    break;

                case EKeyboardKey.Down:
                    if (_selectedRow < _maxRowOnPage)
                        _selectedRow++;
                    break;
            }

            DrawList();
        }

        private void RefreshMapList()
        {
            _mapList = MapFileUtils.GetMapList();
            _mapSelection = 0;
            _selectedRow = 0;
            _currentPage = 1;
            _maxRowOnPage = 0;
            _mapCount = _mapList.Count;
            _totalPages = (int)Math.Ceiling((decimal)_mapCount / (decimal)_pageSize);
            _mapListLoaded = false;
            DrawList();
        }

        private void DrawList()
        {
            if (_mapCount == 0)
            {
                Text = "NO CUSTOM MAPS FOUND\n\nPRESS ANY BUTTON TO CONTINUE...";
                _isError = true;
                return;
            }

            if (_isError)
            {
                return;
            }
            
            var mapText = new StringBuilder()
                .Append("<color=#009933>SELECT MAP WITH ARROWS, LOAD WITH ENTER:</color>")
                .AppendLine();
            
            var startIndex = (_currentPage - 1) * _pageSize;
            var endIndex = Math.Min(startIndex + _pageSize - 1, _mapCount - 1);
            _maxRowOnPage = endIndex - startIndex;
            _selectedRow = Math.Min(_selectedRow, _maxRowOnPage);
            var line = 0;
            for (var i = startIndex; i <= endIndex; i++)
            {
                var mapName = _mapList[i].PackageInfo.Descriptor.Name.Length > 31
                    ? _mapList[i].PackageInfo.Descriptor.Name.Substring(0, 30) + ".."
                    : _mapList[i].PackageInfo.Descriptor.Name;
                mapText.AppendLine($"{(line == _selectedRow ? "<color=#00cc44>>" : "  ")} {mapName}{(line == _selectedRow ? "</color>" : "")}");
                line++;
                _mapSelection = startIndex + _selectedRow;
            }

            for (int j = 0; j < _pageSize - (endIndex - startIndex + 1); j++)
                mapText.AppendLine();


            mapText.Append(_currentPage > 1 ? "<<      " : "         ");
            mapText.Append($"{_currentPage,3}/{_totalPages,-4}");
            mapText.Append(_currentPage < _totalPages ? "      >>" : "        ");
            //"  [ ^/v - selection   </> - change page ]"   {i + 1,2}.
            Text = mapText.ToString();
        }
    }
}