using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;

namespace VmodMonkeMapLoader.Behaviours
{
	public struct RoomInfoData
	{
		public string name;
		public string gameMode;
		public string region;
		public int playerCount;
	}

	public class MonkeRoomManager : MonoBehaviourPunCallbacks
	{
		public static MonkeRoomManager Instance { get; private set; }

		string forcedRegion = "none";
		List<string> checkedRegions;

		List<RoomInfoData> roomListCache = new List<RoomInfoData>();

		Dictionary<string, Dictionary<string, int>> MapsInRegion = new Dictionary<string, Dictionary<string, int>>();

		int updateCounter = 0;

		void Awake()
		{
			Instance = this;
		}

		public override void OnRoomListUpdate(List<RoomInfo> roomList)
		{
			// TODO: Cleanup and remove excessive logging
			//Il2CppString* currentRegionCS = PhotonNetwork::get_PhotonServerSettings()->AppSettings->FixedRegion;
			//std::string currentRegion = currentRegionCS ? to_utf8(csstrtostr(currentRegionCS)) : "";
			string currentRegion = PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion;

			//   if (forcedRegion == "none" && !checkedRegions)
			//{
			//    // clear list because we are gonna add new stuff
			//    checkedRegions = new std::vector<std::string>();
			//    forcedRegion = currentRegion;
			//    Array<Il2CppString*>* regions = PhotonNetworkController::_get_instance()->serverRegions;
			//    for(int i= 0; i < regions->Length(); i++)
			//    {
			//        std::string region = regions->values[i] ? to_utf8(csstrtostr(regions->values[i])) : "";

			//        // we want a list of all regions except current region
			//        if (region != currentRegion) checkedRegions->push_back(region);
			//    }
			//}
			if (forcedRegion == "none" && checkedRegions == null)
			{
				checkedRegions = new List<string>();
				forcedRegion = currentRegion;
				string[] regions = PhotonNetworkController.instance.serverRegions;
				foreach (string region in regions)
				{
					// we want a list of all regions except current region
					if (region != currentRegion) checkedRegions.Add(region);
				}
				Helpers.Logger.LogText($"Current regions: {string.Join(", ", checkedRegions)}");
			}

			//getLogger().info("Sorting through %d rooms for region %s", roomList->get_Count(), currentRegion.c_str());
			//int validRoomscount = 0;
			//int oldRooms = 0;
			Helpers.Logger.LogText($"Sorting through {roomList.Count} rooms for region {currentRegion}");
			int validRoomscount = 0;
			int oldRooms = 0;

			//// foreach in roomlist
			//for (int i = 0; i < roomList->get_Count(); i++)
			//{
			foreach (RoomInfo roomInfo in roomList)
			{
			//    RoomInfo* roomInfo = roomList->items->values[i];
			//    // if nullptr just skip cause crashy crash
			//    if (!roomInfo) continue;
			//    RoomInfoData data(roomInfo, currentRegion);
				if (roomInfo == null) continue;

				RoomInfoData data = new RoomInfoData
				{
					name = roomInfo.Name,
					gameMode = roomInfo.CustomProperties.TryGetValue("gameMode", out var gamemode) ? gamemode as string : "",
					playerCount = roomInfo.PlayerCount,
					region = currentRegion
				};

				//    // if there is no _ in the gamemode, it's not a custom map and can be skipped
				//    if (data.gameMode.find("_") == std::string::npos) continue;
				if (!data.gameMode.Contains("_")) continue;

				//    // will try to find at least 1 room with the same name in our cache
				//    bool found = false;
				//    int foundIndex = 0;
				//    for (int j = 0; j < roomListCache.size(); j++)
				//    {
				bool found = false;
				int foundIndex = 0;
				for (int j = 0; j < roomListCache.Count; j++)
				{
					//        RoomInfoData& infoData = roomListCache[j];
					//        // if you find one that has the same name, just make it true and break since having even one makes it contain
					//        if (infoData.name == data.name)
					//        {
					//            found = true;
					//            foundIndex = j;
					//            break;
					//        }
					//    }

					RoomInfoData infoData = roomListCache[j];
					if (infoData.name == data.name)
					{
						found = true;
						foundIndex = j;
						break;
					}
				}

			//    // if not found (count == 0)
			//    if (!found)
			//    {
			//        validRoomscount++;
			//    }
			//    // if found, replace it (remove)
			//    else
			//    {
			//        oldRooms++;
			//        roomListCache.erase(roomListCache.begin() + foundIndex);
			//    }
				
			//    // we always need to add a new one, so that is what we shall do
			//    roomListCache.push_back(data);
			//}   
				if (!found)
				{
					validRoomscount++;
				} else
				{
					oldRooms++;
					roomListCache.RemoveAt(foundIndex);
				}

				roomListCache.Add(data);
			}

			//// log new room and old room count for completeness sake
			//getLogger().info("%d new Valid Rooms in %s, %d old rooms", validRoomscount, currentRegion.c_str(), oldRooms);
			Helpers.Logger.LogText($"{validRoomscount} new Valid Rooms in {currentRegion}, {oldRooms} old rooms");

			//// reset all room counts for current region
			//RegionToMap::iterator regionIT = MapsInRegion.find(currentRegion);
			//if (regionIT != MapsInRegion.end())
			//{
			//    for (auto& p : regionIT->second)
			//    {
			//        p.second = 0;
			//    }
			//}
			//else
			//{
			//    MapsInRegion[currentRegion] = (MapToCount){};
			//    regionIT = MapsInRegion.find(currentRegion);
			//}

			if (MapsInRegion.TryGetValue(currentRegion, out var regionIT))
			{
				foreach (var room in regionIT.Keys.ToList())
				{
					regionIT[room] = 0;
				}
			} else
			{
				MapsInRegion.Add(currentRegion, new Dictionary<string, int>());
				regionIT = MapsInRegion[currentRegion];
			}

			//// for each map data
			//for (auto& data : roomListCache)
			//{
			foreach (var data in roomListCache)
			{
				//    // if we are looking at a map that is not in the current checked region, skip, otherwise it would be counted towards each region
				//    if (data.region != currentRegion) continue;
				Helpers.Logger.LogText($"Start of for loop {data.gameMode}");
				if (data.region != currentRegion) continue;

				//    // get the map ID from the gamemode string, this is from MOD_ till DEFAULT
				//    int start = data.gameMode.find("MOD_") + 4;
				//    int end = data.gameMode.find("DEFAULT");
				//    std::string get = data.gameMode.substr(start, end - start);
				int start = data.gameMode.IndexOf("MOD_") + 4;
				// TODO: this breaks in casual lobbies
				int end = data.gameMode.IndexOf("DEFAULT");
				if (end == -1)
				{
					end = data.gameMode.IndexOf("CASUAL");
					if (end == -1)
					{
						end = data.gameMode.IndexOf("COMPETITIVE");
						if (end == -1)
						{
							end = data.gameMode.Length - 1;
						}
					}
				}
				string get = data.gameMode.Substring(start, end-start);

				//    getLogger().info("GameMode: %s\n get: %s", data.gameMode.c_str(), get.c_str());
				Helpers.Logger.LogText($"GameMode: {data.gameMode}, \"{get}\"");

				//    // try to find if it exists-
				//   MapToCount::iterator it = regionIT->second.find(get);
				//    // if it does, add to the count there
				//    if (it != regionIT->second.end()) it->second += data.playerCount;
				//    // else just set it
				//    else regionIT->second[get] = data.playerCount;
				if (regionIT.ContainsKey(get))
				{
					regionIT[get] += data.playerCount;
				} else
				{
					regionIT.Add(get, data.playerCount);
				}

				//}

			}

			//   if (roomList->get_Count() > 0)
			//{
			//    // if we are not done checking regions
			//    if (checkedRegions->size() > 0)
			//    {
			//        // get new region
			//        std::string newRegion = checkedRegions->at(0);

			//        // remove first region from the list
			//        checkedRegions->erase(checkedRegions->begin());

			//        // set the value
			//        patchForcedRegion = newRegion;
					
			//        // disconnect makes it reconnect to master
			//        PhotonNetworkController::_get_instance()->AttemptDisconnect();
			//        //PhotonNetwork::Disconnect();
			//    }
			//    else if (checkedRegions->size() == 0 && forcedRegion != "none")
			//    {
			//        // set back to best ping region
			//        patchForcedRegion = forcedRegion;
			//        PhotonNetworkController::_get_instance()->AttemptDisconnect();
			//        forcedRegion = "none";
			//    }
			//}

			if (roomList.Count > 0)
			{
				Helpers.Logger.LogText("Room list count is >0");
				// if we are not done checking regions
				if (checkedRegions.Count > 0)
				{
					Helpers.Logger.LogText($"Checked regions count > 0, {checkedRegions == null}");
					// get new region
					string newRegion = checkedRegions[0];
					checkedRegions.RemoveAt(0);

					// set the value
					Patches.ForceRegionPatch.patchForcedRegion = newRegion;

					// disconnect makes it reconnect to master
					PhotonNetworkController.instance.AttemptDisconnect();
				} else if (checkedRegions.Count == 0 && forcedRegion != "none")
				{
					Helpers.Logger.LogText("Checked regions count = 0");
					// set back to best ping region
					Patches.ForceRegionPatch.patchForcedRegion = forcedRegion;
					PhotonNetworkController.instance.AttemptDisconnect();
					forcedRegion = "none";
				}
			}
			Helpers.Logger.LogText("Got out of room list >0 block");

			//updateCounter() ++;
			//      getLogger().info("update Counter: %d", updateCounter());
			//      // every n cycles, update all room data
			//      if (updateCounter() % 20 == 0)
			//      {
			//          updateCounter() = 0;
			//          getLogger().info("Updating all regions in room list cache");

			//          roomListCache.clear();

			//          checkedRegions->clear();
			//          delete(checkedRegions);
			//          checkedRegions = nullptr;

			//          forcedRegion = "none";

			//          PhotonNetworkController::_get_instance()->AttemptDisconnect();
			//      }

			updateCounter++;

			Helpers.Logger.LogText($"update Counter: {updateCounter}");
			if (updateCounter % 20 == 0)
			{
				updateCounter = 0;
				Helpers.Logger.LogText("Updating all regions in room list cache");

				roomListCache.Clear();

				checkedRegions = null;
				forcedRegion = "none";

				PhotonNetworkController.instance.AttemptDisconnect();
			}
		}

		public override void OnConnectedToMaster()
		{
			if (PhotonNetwork.InLobby)
			{
				PhotonNetwork.LeaveLobby();
			}
			PhotonNetwork.JoinLobby();
		}

		public int PlayersOnMap(string mapName)
		{
			//   int count = 0;
			//bool found = false;
			//for (auto& p : MapsInRegion)
			//{
			//    MapToCount::iterator it = p.second.find(mapName);
			//    if (it != p.second.end()) 
			//    {
			//        found = true;
			//        count += it->second;
			//    }
			//}

			int count = 0;
			bool found = false;
			foreach (var region in MapsInRegion)
			{
				if (region.Value.TryGetValue(mapName, out int value))
				{
					found = true;
					count += value;
				}
			}

			//if (found) 
			//{
			//    getLogger().info("Player count for map %s is %d", mapName.c_str(), count);
			//}
			//else
			//{
			//    getLogger().info("Could not find player count for map %s", mapName.c_str());
			//}

			if (found)
			{
				Helpers.Logger.LogText($"Player count for map {mapName} is {count}");
			} else
			{
				Helpers.Logger.LogText($"Could not find player count for map {mapName}");
			}

			return count;
		}
	}
}
