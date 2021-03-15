using System.Collections;
using System.Text;
using ComputerInterface;
using ComputerInterface.ViewLib;
using UnityEngine;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using Zenject;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MapDetailsView : ComputerView
    {
        private readonly MapLoader _mapLoader;

        private MapInfo _mapInfo;
        private SharedCoroutineStarter _coroutineStarter;
        private bool _isError;
        private bool _isMapLoaded;

        public MapDetailsView(MapLoader mapLoader, SharedCoroutineStarter coroutineStarter)
        {
            _mapLoader = mapLoader;
            _coroutineStarter = coroutineStarter;
        }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);

            if (args == null || args.Length == 0)
            {
                Text = "No map selected;";
                _isError = true;
                return;
            }

            var mapInfo = args[0] as MapInfo ?? Constants.MapInfoError;

            _isError = false;
            _isMapLoaded = false;
            _mapInfo = mapInfo;
            PrintMapInfo();
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (_isError)
            {
                _isError = false;
                ShowView<MapListView>();
                return;
            }

            if (_isMapLoaded)
            {
                _isMapLoaded = false;
                PrintMapInfo();
                return;
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    _mapInfo = null;
                    ShowView<MapListView>();
                    break;

                case EKeyboardKey.Enter:
                    if (_isMapLoaded)
                        break;
                    Text = "Loading map: " + _mapInfo.PackageInfo.Descriptor.Name;
                    _mapLoader.LoadMap(_mapInfo, b => OnMapLoaded());
                    break;
            }
        }

        private void PrintMapInfo()
        {
            var mapDescriptor = _mapInfo.PackageInfo.Descriptor;
            var sb = new StringBuilder()
                .AppendClr("<noparse> << [BACK]            [ENTER]  LOAD MAP</noparse>", "8dc2ef").AppendLine()
                .AppendLine()
                .AppendLine("MAP DETAILS")
                .AppendLine()
                .Append("NAME:  ").AppendClr(mapDescriptor.Name, "00cc44").AppendLine()
                .Append("AUTHOR:  ").AppendClr(mapDescriptor.Author, "00cc44").AppendLine()
                .Append("DESCRIPTION:  ").AppendClr(mapDescriptor.Description, "00cc44").AppendLine();

            Text = sb.ToString();
        }

        private void OnMapLoaded()
        {
            Text = "\n\n\n<align=\"center\">Map loaded!";
            _isMapLoaded = true;

            _coroutineStarter.StopCoroutine(HideMapLoaderText());
            _coroutineStarter.StartCoroutine(HideMapLoaderText());
        }

        private IEnumerator HideMapLoaderText()
        {
            yield return new WaitForSeconds(5f);

            if (_isMapLoaded)
            {
                _isMapLoaded = false;
                PrintMapInfo();
            }
        }
    }
}