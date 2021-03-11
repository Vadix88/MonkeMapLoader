# Monke Map Loader

Custom map loader mod for a VR game **Gorilla Tag** (PC version).

Initially, it started as a solo project to try to load a custom map to the game, for later to become a group effort to provide a complex solution with a nice in-game interface for selecting maps to load, auto joining private rooms to play with others on selected map, a set of tools for map makers to use in their creations to add more variety to the gameplay and a set up and ready to use Unity project with map export script.

Main contributors beside me:

- [Bobbie](https://github.com/legoandmars)
- [Steven](https://github.com/DeadlyKitten)
- [RedBrumbler](https://github.com/RedBrumbler)

You can find all of us, as well as other mods, on the [Gorilla Tag Modding Discord](http://discord.gg/b2MhDBAzTv)

**Contents**

  - [Installation](#installation)
  - [Custom maps](#custom-maps)
  - [Loading a map](#loading-a-map)
  - [Troubleshooting](#troubleshooting)
  - [Map making](#map-making)

## Installation

### Automatic

You can install **Monke Map Loader** automatically with the help of [Monke Mod Manager](https://github.com/DeadlyKitten/MonkeModManager/releases/latest). It will make sure that you have the mod loader and all the needed dependencies installed.

### Manual

1. **Monke Map Loader** requires BepInEx mod loader installed to work. You can download it from [this page](https://github.com/BepInEx/BepInEx/releases). From the *Assets* dropdown pick the appropriate version for your operating system (for example: *BepInEx_x64_VERSION.zip* for Windows 10 64-bit). Then extract the zip file to the main folder of the game, for example to:  
*C:\\Program Files\\Steam\\steamapps\\common\\Gorilla Tag\\*  
Run the game once and close it for the BepInEx to install itself and create the folder structure.

1. Download the **Monke Map Loader** from the latest [release]() and extract the zip file into the games folder. Zip package already contains the folder structure, so all files should end up in the right location.

1. Download all the dependencies and put them in suitable locations, according to the installation guide of each:
   - [Utilla](https://github.com/legoandmars/Utilla/releases/latest)
   - [ComputerInterface](https://github.com/ToniMacaroni/ComputerInterface/releases/latest)
   - [Bepinject and Extenject](https://github.com/Auros/Bepinject/releases/latest)
   - [Newtonsoft.Json](https://github.com/legoandmars/Newtonsoft.Json/releases/latest)

## Custom maps

All the custom maps should be placed inside the folder:  
> *BepInEx\\plugins\\MonkeMapLoader\\CustomMaps*

**Monke Map Loader** uses a custom **Gorilla Tag Map** file format (*.gtmap*), which contains a package of descriptor file, map thumbnails and map files for both platforms: Windows and Android (Quest), therefore it's compatible with both PC and Quest.

## Loading a map

**Monke Map Loader** is using the ComputerInterface mod to provide a comfortable way of selecting the map you want to load:
1. On the main menu select the **Monke Map Loader** with the arrow keys and hit *Enter*.
2. You'll see the list of all maps found in the CustomMaps folder. You can select one using *Up* and *Down* arrow keys or switch the page with *Left* and *Right* keys. Then select the desired map by pressing *Enter*.
3. On the next screen are shown the details of the chosen map, like its Title, Author's name and Description. To confirm the choice hit *Enter* again (you can go back to previous screens using the *Back* key).
5. After the loading is complete you should see the confirmation on the screen.
6. You can now step into the teleporter on the left of the computer and after a brief delay you'll be teleported to the chosen map.
7. Because using mods in public rooms/lobbies in not allowed, **Monke Mod Loader** uses a private room matchmaking system, which will auto join you to a private room with other players that have the same map loaded.
8. To load a different map you need to go back to the tree room/base using the teleporter placed somewhere on the map (it's up to the map maker where it will be), and then select another map on the computer.

Custom maps are loaded in different location from the original map so it stays intact and can be still used to play on it.

## Troubleshooting

- If the **Monke Map Loader** mod is not showing in the main computer menu it can lack some of the dependencies or it can be incorrectly installed. Check the installation steps or use the Monke Mod Manager to install it for you.
- If the map isn't showing on the list it may be in a wrong file format or have incorrect descriptor file. It's best to use the provided [Unity project](https://github.com/legoandmars/GorillaTagMapProject) to make/extract the map to the proper format.
- If you encounter any bugs/errors with the operation of map loader please contact Vadix or Bobbie on the [Discord server](http://discord.gg/b2MhDBAzTv).
- If you encounter bugs on the map itself try contacting the map Author and give him you feedback.

## Map making

Custom maps should be made in Unity editor version: **2019.3.15f1** available for download in [Unity's archive](https://unity3d.com/get-unity/download/archive). Using the Unity Hub is adviced to make managing versions easier. **Make sure to install the Android build support through the installation process!**

The easiest and recommended way of making the map in the right format is to use the [Gorilla Tag Map Project](https://github.com/legoandmars/GorillaTagMapProject) which is an already set up Unity project with map tools prefabs and an export script, which automates the making of the map package and saves it in the right format. On that page you can also find the in-depth guide how to use it in its full potential and what tools and objects are available to use on your map to add extra features like teleports.

Have fun making you own custom map for this awesome game and share it with others! ;)