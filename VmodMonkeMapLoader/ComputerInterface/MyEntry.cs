using ComputerInterface.Interfaces;
using System;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MyEntry : IComputerModEntry
    {
        public string EntryName => "MyMod";

        // This is the first view that is going to be shown if the user select you mod
        // The Computer Interface mod will instantiate your view 
        public Type EntryViewType => typeof(View2);
    }
}