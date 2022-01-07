using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using GorillaNetworking;

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
			string currentRegion = PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion;

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
				//Helpers.Logger.LogText($"Current regions: {string.Join(", ", checkedRegions)}");
			}

			//Helpers.Logger.LogText($"Sorting through {roomList.Count} rooms for region {currentRegion}");
			int validRoomscount = 0;
			int oldRooms = 0;

			foreach (RoomInfo roomInfo in roomList)
			{
				if (roomInfo == null) continue;

				RoomInfoData data = new RoomInfoData
				{
					name = roomInfo.Name,
					gameMode = roomInfo.CustomProperties.TryGetValue("gameMode", out var gamemode) ? gamemode as string : "",
					playerCount = roomInfo.PlayerCount,
					region = currentRegion
				};

				// if there is no _ in the gamemode, it's not a custom map and can be skipped
				if (!data.gameMode.Contains("_")) continue;

				// will try to find at least 1 room with the same name in our cache
				bool found = false;
				int foundIndex = 0;
				for (int j = 0; j < roomListCache.Count; j++)
				{
					// if you find one that has the same name, just make it true and break since having even one makes it contain
					RoomInfoData infoData = roomListCache[j];
					if (infoData.name == data.name)
					{
						found = true;
						foundIndex = j;
						break;
					}
				}
				
				if (!found)
				{
					validRoomscount++;
				} else
				{
					oldRooms++;
					roomListCache.RemoveAt(foundIndex);
				}

				// we always need to add a new one, so that is what we shall do
				roomListCache.Add(data);
			}

			// log new room and old room count for completeness sake
			if (validRoomscount > 0) Helpers.Logger.LogText($"{validRoomscount} new Valid Rooms in {currentRegion}, {oldRooms} old rooms");

			// reset all room counts for current region
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

			// for each map data
			foreach (var data in roomListCache)
			{
				// if we are looking at a map that is not in the current checked region, skip, otherwise it would be counted towards each region
				if (data.region != currentRegion) continue;

				// get the map ID from the gamemode string, this is from MOD_ till DEFAULT
				int start = data.gameMode.IndexOf("MOD_") + 4;
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

				// try to find if it exists
				if (regionIT.ContainsKey(get))
				{
					// if it does, add to the count there
					regionIT[get] += data.playerCount;
				} else
				{
					// else just set it
					regionIT.Add(get, data.playerCount);
				}
			}

			if (roomList.Count > 0)
			{
				// if we are not done checking regions
				if (checkedRegions.Count > 0)
				{
					// get new region
					string newRegion = checkedRegions[0];
					checkedRegions.RemoveAt(0);

					// set the value
					Patches.ForceRegionPatch.patchForcedRegion = newRegion;

					// disconnect makes it reconnect to master
					PhotonNetworkController.instance.AttemptDisconnect();
				} else if (checkedRegions.Count == 0 && forcedRegion != "none")
				{
					// set back to best ping region
					Patches.ForceRegionPatch.patchForcedRegion = forcedRegion;
					PhotonNetworkController.instance.AttemptDisconnect();
					forcedRegion = "none";
				}
			}

			updateCounter++;

			//Helpers.Logger.LogText($"update Counter: {updateCounter}");
			// every n cycles, update all room data
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
