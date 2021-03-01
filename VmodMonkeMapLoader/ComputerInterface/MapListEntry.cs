using ComputerInterface.Interfaces;
using System;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MapListEntry : IComputerModEntry
    {
        public string EntryName => "Vmod - Monke Map Loader";
        
        public Type EntryViewType => typeof(MapListView);
    }
}