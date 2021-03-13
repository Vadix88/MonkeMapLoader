using System.Text;
using ComputerInterface;
using ComputerInterface.ViewLib;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using Zenject;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MapDetailsView : ComputerView
    {
        [Inject]
        public MapLoader MapLoader { get; set; }

        private MapInfo _mapInfo;
        private bool _isError = false;

        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            if (args == null || args.Length == 0)
            {
                Text = "No map selected;";
                _isError = true;
                return;
            }

            var mapInfo = args[0] as MapInfo;
            if (mapInfo == null)
            {
                mapInfo = Constants.MapInfoError;
            }

            _isError = false;
            _mapInfo = mapInfo;
            PrintMapInfo();
        }

        public override void OnKeyPressed(EKeyboardKey key)
        {
            if (_isError)
            {
                _isError = false;
                //ReturnView();
                ShowView<MapListView>();
                return;
            }

            switch (key)
            {
                case EKeyboardKey.Back:
                    //ReturnView();
                    _mapInfo = null;
                    ShowView<MapListView>();
                    break;

                case EKeyboardKey.Enter:
                    if (_isError)
                        break;
                    Text = "Loading map: " + _mapInfo.PackageInfo.Descriptor.Name;
                    MapLoader.LoadMap(_mapInfo, b => OnMapLoaded());
                    break;
            }
        }

        private void PrintMapInfo()
        {
            var mapDescriptor = _mapInfo.PackageInfo.Descriptor;
            var sb = new StringBuilder()
                .AppendLine("<noparse> << BACK            ENTER - LOAD MAP</noparse>")
                .AppendLine()
                .AppendLine("MAP DETAILS")
                .AppendLine()
                .Append("NAME:  <color=#00cc44>").Append(mapDescriptor.Name).AppendLine("</color>")
                .Append("AUTHOR:  <color=#00cc44>").Append(mapDescriptor.Author).AppendLine("</color>")
                .Append("DESCRIPTION:  <color=#00cc44>").Append(mapDescriptor.Description).AppendLine("</color>");

            Text = sb.ToString();
        }

        private void OnMapLoaded()
        {
            Text = "\n\n\n<align=\"center\">Map loaded!";
            _isError = true;
        }
    }
}