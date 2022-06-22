using ComputerInterface.Interfaces;
using System;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MapBrowseEntry : IComputerModEntry
    {
        public string EntryName => "Monke Map Hub Browser";
        
        public Type EntryViewType => typeof(MapBrowseView);
    }
}