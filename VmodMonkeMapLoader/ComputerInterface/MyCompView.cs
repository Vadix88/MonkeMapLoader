using ComputerInterface;
using ComputerInterface.ViewLib;
using VmodMonkeMapLoader.Behaviours;
using VmodMonkeMapLoader.Models;
using Zenject;

namespace VmodMonkeMapLoader.ComputerInterface
{
    public class MyCompView : ComputerView
    {
        [Inject]
        public MapLoader MapLoader { get; set; }

        public override void OnShow(object[] args)
        {
            base.OnShow(args);
            // changing the Text property will fire an PropertyChanged event
            // which lets the computer know the text has changed and update it
            Text = "Beep Boop Press ENTER";
        }

        // you can do something on keypresses by overriding "OnKeyPressed"
        // it get's an EKeyboardKey passed as a parameter which wraps the old character string
        public override void OnKeyPressed(EKeyboardKey key)
        {
            switch (key)
            {
                case EKeyboardKey.Delete:
                    // "ReturnView" will basically "go back" and return to the last opened view
                    ReturnView();
                    break;

                case EKeyboardKey.Enter:
                    MapLoader.LoadMap(new MapInfo
                    {
                        FilePath =
                            @"C:\gry\Steam\steamapps\common\Gorilla Tag\BepInEx\plugins\VmodMonkeMapLoader\CustomMaps\testmap6.zip",
                        PackageInfo = new MapPackageInfo
                        {
                            PcFileName = @"maptest6",
                            Descriptor = new MapDescriptor
                            {
                                Name = "Test map 6"
                            },
                            Config = new MapConfig
                            {
                                RootObjectName = "Origin"
                            }
                        }
                    }, b => Text = "SUCCESS!!!!!!!!");
                    break;
            }
        }
    }
}