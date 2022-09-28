# Monke Map Loader

<img src="https://user-images.githubusercontent.com/34404266/115944749-8cc3d680-a46c-11eb-9298-61d866b687fb.png" data-canonical-src="https://user-images.githubusercontent.com/34404266/115944749-8cc3d680-a46c-11eb-9298-61d866b687fb.png" width="500"/>

A PC mod that loads custom maps for Gorilla Tag.

**Monke Map Loader** initially started as a solo project to try to load a custom map into the game. Eventually, it became a group effort to provide a complex solution with:

- A nice in-game interface for selecting maps
- Fake "public lobby" matchmaking to play with others on custom maps
- Sets of tools for map makers to use in their creations to add gameplay variety
- A ready to use Unity project with an automatic map export script.

Main contributors beside me:

- [Bobbie](https://github.com/legoandmars)
- [Steven](https://github.com/DeadlyKitten)
- [RedBrumbler](https://github.com/RedBrumbler)
- [ToniMacaroni](https://github.com/ToniMacaroni)
- [Graic](https://github.com/Graicc)

You can find all of us, as well as other mods, on the [Gorilla Tag Modding Discord](http://discord.gg/b2MhDBAzTv)

## Contents

- [Installation](#installation)
- [Custom maps](#custom-maps)
- [Loading a map](#loading-a-map)
- [Troubleshooting](#troubleshooting)
- [Map making](#map-making)
- [For Developers](#for-developers)

## Installation

### Automatic

You can install **Monke Map Loader** automatically with the help of [Monke Mod Manager](https://github.com/DeadlyKitten/MonkeModManager/releases/latest). It will make sure that you have the map loader and all the needed dependencies installed.

### Manual

**Monke Map Loader** requires BepInEx to work. You can download it from [this page](https://github.com/BepInEx/BepInEx/releases). From the *Assets* dropdown pick the appropriate version for your operating system (for example: *BepInEx_x64_VERSION.zip* for Windows 10 64-bit).

Then, extract the zip file to the main folder of the game, for example to:  
*C:\\Program Files\\Steam\\steamapps\\common\\Gorilla Tag\\*  

Run the game once and close it for the BepInEx to install itself and create the proper folder structure.

Download all the dependencies and put them in suitable locations, according to the installation guide of each:

- [Utilla](https://github.com/legoandmars/Utilla/releases/latest)
- [ComputerInterface](https://github.com/ToniMacaroni/ComputerInterface/releases/latest)
- [Bepinject and Extenject](https://github.com/Auros/Bepinject/releases/latest)
- [Newtonsoft.Json](https://github.com/legoandmars/Newtonsoft.Json/releases/latest)

Download the **Monke Map Loader** from the latest [release](https://github.com/Vadix88/VmodMonkeMapLoader/releases/latest) and extract the zip file into the games folder.

## Custom maps

You can download custom maps from our website: [Monke Map Hub](https://monkemaphub.com/).

To install a custom map, place it in the following folder:
> *BepInEx\\plugins\\MonkeMapLoader\\CustomMaps*

**Monke Map Loader** uses a custom **Gorilla Tag Map** file format (*.gtmap*), which contains a package of a descriptor file, map thumbnails and map files for both platforms: Windows and Android (Quest)

Therefore, `.gtmap` files are also compatible with Quest map loader.

## Loading a map

**Monke Map Loader** uses the ComputerInterface mod to provide a comfortable way of selecting the map you want to load:

1. On the main menu select the **Monke Map Loader** with the arrow keys and hit *Enter*.
2. You'll see the list of all installed custom maps with an orb next to the PC to preview the current level. You can select one using *Up* and *Down* arrow keys or switch the page with *Left* and *Right* keys. Then select the desired map by pressing *Enter*.
3. On the next screen there will be details of the chosen map, such as the Title, Author Name and Description. To confirm the choice hit *Enter* again (you can go back to previous screens using the *Back* key).
4. After the loading is complete you should see a confirmation on the screen.

You can now step into the teleporter on the left of the computer and after a brief delay you'll be teleported to the chosen map.

Because using mods in public rooms/lobbies in not allowed, **Monke Mod Loader** uses a private room matchmaking system, which will auto join you to a private room with other players that have the same map loaded.

To load a different map, you'll need to go back to the tree room/base using a green teleporter placed somewhere on the map, and then select another map on the computer.

Custom maps are loaded in different location from the original map, so it stays intact and can be still used to play on it.

## Troubleshooting

- If the **Monke Map Loader** mod is not showing in the main computer menu, some dependencies may not be installed. Check the installation steps or use the Monke Mod Manager to install it for you.
- If the map isn't showing on the list it may be in a wrong file format or be corrupt. If you're the map creator, it's best to use the provided [Unity project](https://github.com/legoandmars/GorillaTagMapProject) to make/extract the map to the proper format.
- If you encounter any bugs/errors with the operation of map loader please contact Vadix or Bobbie on the [Discord server](http://discord.gg/b2MhDBAzTv).
- If you encounter bugs on specific maps try contacting the map Author to give them feedback.

## Map making

The easiest and recommended way of making the map in the right format is to use the [Gorilla Tag Map Project](https://github.com/legoandmars/GorillaTagMapProject), an pre-setup Unity project with map tools, prefabs, and an export script that automates the making of the map package and saves it in the right format.

On that page you can also find an in-depth guide on how to use to its full potential and what tools and objects are available to use on your map. **MAKE SURE TO FOLLOW THE GUIDE IN THE README!**

You can also get more help on map making on the [Discord server](http://discord.gg/b2MhDBAzTv).

Have fun making you own custom map for this awesome game and make sure to share it with others! ;)

## For Developers

The map loader allows other mods to access some info about the current map (including arbitrary custom data) and subscribe to actions for map load/unload and map join/leave. All public events are under the [Events class](/MonkeMapLoader/Events.cs).

```cs
public class MyMod : MonoBehaviour {
  public void Awake() {
    Events.OnMapEnter += OnMapEnter;
    Events.OnMapChange += OnMapChange;
  }

  // Going through the teleporter
  public void OnMapEnter(bool enter) {
    if (enter) {
      // Get the custom map data
      var customData = Events.PackageInfo.Config.CustomData;

      // Check if the gamemode is "mygamemode"
      if (customData.TryGetValue("gamemode", out var gamemode) && gamemode as string == "myGameMode") {
        // Get instance of MyClass from custom data
        MyClass myData = (customData["mydata"] as Newtonsoft.Json.Linq.JObject).ToObject<MyClass>();
      }

      // Get other data
      Debug.Log($"Map name is: {Events.MapName}");
      Debug.Log($"Gravity speed is: {Events.Descriptor.GravitySpeed}");
    }
  }

  // Map creation / destruction
  public void OnMapChange(bool enter) {
    if (!enter) {
      RemoveMyObjects();
    }
  }
}
```

Documentation for adding custom data to a map can be found in the [Gorilla Tag Map Project](https://github.com/legoandmars/GorillaTagMapProject#for-developers) readme.

## Disclaimers
This product is not affiliated with Gorilla Tag or Another Axiom LLC and is not endorsed or otherwise sponsored by Another Axiom LLC. Portions of the materials contained herein are property of Another Axiom LLC. ©2021 Another Axiom LLC.
