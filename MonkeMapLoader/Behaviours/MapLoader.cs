using BepInEx;
using GorillaNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using Zenject;
using Logger = VmodMonkeMapLoader.Helpers.Logger;
using Object = UnityEngine.Object;

namespace VmodMonkeMapLoader.Behaviours
{
    public class MapLoader : IInitializable
    {
		public static string _lobbyName;

        private static GameObject _mapInstance;
        private static bool _isLoading;
        internal static GlobalData _globalData;
        internal static MapDescriptor _descriptor;
        internal static MapInfo _mapInfo;
        private static bool isMoved = false;
        internal static string _mapFileName;

        private SharedCoroutineStarter _couroutineStarter;

        private static GameObject _forest;
		private HttpClient _client;

		[Inject]
        private void Construct(SharedCoroutineStarter coroutineStarter, HttpClient client)
        {
            _couroutineStarter = coroutineStarter;
            _client = client;
        }

        public void Initialize()
        {
            InitializeGlobalData();

            GorillaComputer.instance.gameObject.AddComponent<MonkeRoomManager>();
        }

        public static void ForceRespawn()
        {
            Teleporter treeTeleporter = _globalData.BigTreeTeleportToMap.GetComponent<Teleporter>();

            treeTeleporter.TeleportPoints = GameObject.Find("SpawnPointContainer")?.GetComponentsInChildren<Transform>().ToList();

            var destination = treeTeleporter.TeleportPoints.Count > 1
            ? treeTeleporter.TeleportPoints[UnityEngine.Random.Range(0, treeTeleporter.TeleportPoints.Count)]
            : treeTeleporter.TeleportPoints[0];

            Logger.LogText("Teleporting due to round end");

            Patches.PlayerTeleportPatch.TeleportPlayer(destination);
            
        }

        public static void ColorTreeTeleporter(Color color)
        {
            foreach(Renderer renderer in _globalData.BigTreeTeleportToMap.GetComponentsInChildren<Renderer>())
            {
                foreach(Material mat in renderer.sharedMaterials)
                {
                    if (mat.name.Contains("Center")) mat.SetColor("_Color", color);
                }
            }
        }

        public static void ShowTreeTeleporter()
		{
            _globalData.BigTreeTeleportToMap.SetActive(true);
		}

        public static void HideTreeTeleporter()
		{
            _globalData.BigTreeTeleportToMap.SetActive(false);
		}

        public static void JoinGame()
        {
            try
            {
                AdjustLighting(_mapInstance);
            }
            catch (Exception e)
            {
                Debug.Log("ERROR LOADING TEXTURES");
                Debug.Log(e);
            }

            if (!_lobbyName.IsNullOrWhiteSpace())
            {
                Utilla.Utils.RoomUtils.JoinModdedLobby(_lobbyName, _descriptor.GameMode.ToLower() == "casual");
                if (_descriptor != null)
				{
					if(_descriptor.GravitySpeed != SharedConstants.Gravity)
					{
						Physics.gravity = new Vector3(0, _descriptor.GravitySpeed, 0);
					}

                    // We need to wait for GorillaTagManager to be instanciated,
                    // which is after we connect to a server.
                    Patches.PlayerMoveSpeedPatch.SetSpeed(_descriptor);
				}
                if (!isMoved)
                {
                    isMoved = true;
                    foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.transform.parent == null))
                    {
                        go.transform.position -= new Vector3(0, 5000, 0);
                    }
                }

                // Increase the clipping plane
                Camera.main.farClipPlane += 500;

                // Disable forest
                _forest = _forest ?? GameObject.Find(Constants.ForestPath);
				_forest?.SetActive(false);
            }
		}

        public static void ResetMapProperties()
        {
            if (Physics.gravity.y != SharedConstants.Gravity) Physics.gravity = new Vector3(0, SharedConstants.Gravity, 0);

            //if (GorillaTagManager.instance != null)
            //{
            //	GorillaTagManager.instance.slowJumpLimit = SharedConstants.SlowJumpLimit;
            //	GorillaTagManager.instance.slowJumpMultiplier = SharedConstants.SlowJumpMultiplier;
            //	GorillaTagManager.instance.fastJumpLimit = SharedConstants.FastJumpLimit;
            //	GorillaTagManager.instance.fastJumpMultiplier = SharedConstants.FastJumpMultiplier;
            //}

            //GorillaLocomotion.Player.Instance.maxJumpSpeed = SharedConstants.SlowJumpLimit;
            //GorillaLocomotion.Player.Instance.jumpMultiplier = SharedConstants.SlowJumpMultiplier;

			// Decrease the clipping plane
			Camera.main.farClipPlane -= 500;

            // Enable forest
			_forest = _forest ?? GameObject.Find(Constants.ForestPath);
			_forest?.SetActive(true);

            if (isMoved)
            {
                isMoved = false;
                foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>().Where(x => x.transform.parent == null))
                {
                    go.transform.position += new Vector3(0, 5000, 0);
                }

            }
        }

        public void LoadMap(MapInfo mapInfo, Action<bool> isSuccess)
        {
            _couroutineStarter.StartCoroutine(LoadMapFromPackageFileAsync(mapInfo, b =>
            {
                Logger.LogText("______ MAP LOADED");
                _mapInfo = mapInfo;
                _mapFileName = Path.GetFileNameWithoutExtension(mapInfo.FilePath);
                isSuccess(b);

                if (Events.OnMapChange != null)
				{
                    try
					{
                        Events.OnMapChange(true);
					}
                    catch (Exception e)
					{
                        Debug.LogError(e);
					}
				}
            }));
        }

        public IEnumerator LoadMapFromPackageFileAsync(MapInfo mapInfo, Action<bool> isSuccess)
        {
            if (_isLoading)
            {
                yield break;
            }

            _isLoading = true;
            _lobbyName = "";
            _mapFileName = null;

            UnloadMap();
            Logger.LogText("Loading map: " + mapInfo.FilePath + " -> " + mapInfo.PackageInfo.Descriptor.Name);

            var bundleDataStream = MapFileUtils.GetMapDataStreamFromZip(mapInfo);
            if (bundleDataStream == null)
            {
                Logger.LogText("Bundle not found in package");

                _isLoading = false;
                isSuccess(false);
                yield break;
            }

            var loadBundleRequest = AssetBundle.LoadFromStreamAsync(bundleDataStream);
            yield return loadBundleRequest;

            var bundle = loadBundleRequest.assetBundle;
            if (bundle == null)
            {
                Logger.LogText("Bundle NOT LOADED");

                _isLoading = false;
                isSuccess(false);
                yield break;
            }

            Logger.LogText("Bundle loaded");

            var assetNames = bundle.GetAllAssetNames();

            Logger.LogText("Asset count: " + assetNames.Length + ", assets: " + string.Join(";", assetNames));
            
            Logger.LogText("Asset name: " + mapInfo.PackageInfo.Descriptor.Name);

            string[] scenePath = bundle.GetAllScenePaths();

            if (scenePath.Length <= 0)
            {
                Logger.LogText("Bundle NOT LOADED");

                _isLoading = false;
                isSuccess(false);
                yield break;
            }

            LoadSceneParameters loadSceneParams = new LoadSceneParameters
            {
                loadSceneMode = LoadSceneMode.Additive,
                localPhysicsMode = LocalPhysicsMode.None
            };
            var scene = SceneManager.LoadSceneAsync(scenePath[0], loadSceneParams);
            yield return scene;

            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            MapDescriptor descriptor = Object.FindObjectOfType<MapDescriptor>();

            foreach(GameObject gameObject in allObjects)
            {
                if (gameObject.scene.name != SceneManager.GetActiveScene().name && gameObject.scene.name != "DontDestroyOnLoad")
                {
                    if(gameObject.transform.parent == null & gameObject.transform != descriptor.transform)
                    {
                        gameObject.transform.SetParent(descriptor.transform);
                    }
                }
            }
            GameObject map = descriptor.gameObject;

            if (map == null)
            {
                _isLoading = false;
                isSuccess(false);
                bundle.Unload(false);
                yield break;
            }

            Logger.LogText("Map asset loaded: " + map.name);
            _lobbyName = mapInfo.GetLobbyName();

            Exception ex = null;

            yield return ProcessAndInstantiateMap(map).AsIEnumerator(exception => ex = exception);
            yield return null;

            if (ex != null)
            {
                Logger.LogException(ex);

                isSuccess(false);
                _isLoading = false;
                bundle.Unload(false);
                yield break;
            }

            bundle.Unload(false);
            bundleDataStream.Close();

            _isLoading = false;
            isSuccess(true);
        }

        public void UnloadMap()
        {
            if (_mapInstance != null)
            {
                Logger.LogText("Destroying old map");

                Object.Destroy(_mapInstance);

                _mapInstance = null;

                if (Events.OnMapChange != null)
				{
                    try
					{
                        Events.OnMapChange(false);
					}
                    catch (Exception e)
					{
                        Debug.LogError(e);
					}
				}

                Resources.UnloadUnusedAssets();
            }
        }

		public bool IsMapDownloaded(MonkeMapHubResponse.Map map)
		{
            var mapInfos = MapFileUtils.GetMapListCached();
            foreach (var info in mapInfos)
			{
				if ((info.PackageInfo.Config.GUID == map.MapGuid)
				 || (info.PackageInfo.Descriptor.Name == map.MapName && info.PackageInfo.Descriptor.Author == map.AuthorName))
				{
					return true;
				}
			}
			
            return false;
		}

		public async void DownloadMap(MonkeMapHubResponse.Map map, Action<bool> isSuccess)
		{
            try
			{
				var path = Path.Combine(Path.GetDirectoryName(typeof(MapFileUtils).Assembly.Location), Constants.CustomMapsFolderName, map.MapFileName);
                Debug.LogWarning(path);
				if (File.Exists(path))
				{
					Logger.LogText("Map already downloaded");
				}

				var response = await _client.GetStreamAsync(Constants.MonkeMapHubBase + map.MapFileUrl);
				using (var fileStream = File.OpenWrite(path))
				{
					await response.CopyToAsync(fileStream);
				}

                await _client.GetAsync($"{Constants.MonkeMapHubBase}api/maps/downloaded/{map.MapGuid}");
			}
			catch (Exception e)
			{
				Logger.LogException(e);
				isSuccess(false);
                return;
			}

            isSuccess(true);
		}

		private async Task ProcessAndInstantiateMap(GameObject map)
        {
            await Task.Factory.StartNew(() =>
            {
                Logger.LogText("Instantiate map");

                _mapInstance = map;
                _descriptor = _mapInstance?.GetComponent<MapDescriptor>();

                ProcessChildObjects(map);

                Transform fakeSkybox = _mapInstance.transform.Find("FakeSkybox");
                if (fakeSkybox != null)
                {
                    Material oldMat = fakeSkybox.GetComponent<Renderer>().material; 
                    if (oldMat.HasProperty("_Tex"))
                    {
                        oldMat.SetTexture("_Tex", Resources.Load<Texture2D>("objects/newsky/materials/day"));
                        oldMat.SetColor("_Color", new Color(1, 1, 1, 1));
                    }
                }

                GameObject FallEmergencyTeleport = new GameObject("FallEmergencyTeleport");
                FallEmergencyTeleport.layer = Constants.MaskLayerHandTrigger;
                FallEmergencyTeleport.AddComponent<BoxCollider>().isTrigger = true;
                FallEmergencyTeleport.transform.SetParent(_mapInstance.transform);
                FallEmergencyTeleport.transform.localScale = new Vector3(2000f, 1f, 2000f);
                FallEmergencyTeleport.transform.localPosition = new Vector3(0f, -300f, 0f);

                Teleporter emergencyFallTeleporter = FallEmergencyTeleport.AddComponent<Teleporter>();
                emergencyFallTeleporter.TeleportPoints = _mapInstance.GetComponent<MapDescriptor>().SpawnPoints.ToList();
                emergencyFallTeleporter.TagOnTeleport = true;
                emergencyFallTeleporter.TeleporterType = TeleporterType.Map;
            });
        }

        private void ProcessChildObjects(GameObject parent)
        {
            if (parent == null) return;
            // Logger.LogText("Processing parent: " + parent.name + ", childs: " + parent.transform.childCount);

            for (var i = 0; i < parent.transform.childCount; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;

                SetupCollisions(child);

                if (child.transform.childCount > 0)
                {
                    ProcessChildObjects(child);
                }
            }

            foreach(Teleporter teleporter in parent.gameObject.GetComponentsInChildren<Teleporter>())
            {
                if (teleporter.TeleportPoints == null || teleporter.TeleportPoints.Count == 0)
                {
                    teleporter.TeleportPoints = new List<Transform>() { _globalData.BigTreePoint.transform };
                    teleporter.TeleporterType = TeleporterType.Treehouse;
                }
            }
        }

        private static void AdjustLighting(GameObject child)
        {
            if(child != null)
            {
                var renderers = child.GetComponentsInChildren<Renderer>();
                if (renderers == null) return;
                foreach(var renderer in renderers)
                {
                    if (renderer == null) return;
                    foreach(var material in renderer.materials)
                    {
                        if (material == null) return;
                        try
						{
                            LightingUtils.SetLightingStrength(material, 0.9f);
						} catch (Exception e)
						{
                            Helpers.Logger.LogException(e);
						}
                    }
                }
            }
        }

        private void SetupCollisions(GameObject child)
        {
            if (child == null) return;
            var colliders = child.GetComponents<Collider>();
            if (colliders == null) return;

            // Fix material sound
            var surfaceOverride = child.GetComponent<GorillaSurfaceOverride>();
            if (surfaceOverride == null)
			{
                surfaceOverride = child.AddComponent<GorillaSurfaceOverride>();
                surfaceOverride.overrideIndex = 0;
			}

            foreach (var collider in colliders)
            {
                if (collider == null) return;
                if (collider.isTrigger)
                {
                    child.layer = Constants.MaskLayerPlayerTrigger;
                    break;
                }else if(child.layer == 0)
                {
                    child.layer = 9;
                }
                if (child.GetComponent<Teleporter>() != null
                    || child.GetComponent<TagZone>() != null
                    || child.GetComponent<ObjectTrigger>() != null)
                {
                    collider.isTrigger = true;
                    child.layer = Constants.MaskLayerPlayerTrigger;
                }
            }
        }

        private void InitializeGlobalData()
        {
            if(_globalData == null)
                _globalData = new GlobalData();

            _globalData.Origin = GameObject.Find("slide")?.transform.position ?? Vector3.zero;

            _globalData.TreeOrigin = GameObject.Find("stool")?.transform.position ?? Vector3.zero;
            
            // Tree Teleport Stuff
            if (_globalData.BigTreeTeleportToMap == null)
            {
                var teleporterPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(MapFileUtils).Assembly.Location), Constants.MiscObjectsFolderName, "Teleporter");
                AssetBundle bundle = MapFileUtils.GetAssetBundleFromZip(teleporterPath);
                _globalData.BigTreeTeleportToMap = Object.Instantiate(bundle.LoadAsset<GameObject>("_Teleporter"));

                _globalData.BigTreeTeleportToMap.layer = Constants.MaskLayerPlayerTrigger;
                Object.DontDestroyOnLoad(_globalData.BigTreeTeleportToMap);
                _globalData.BigTreeTeleportToMap.transform.position += new Vector3(0, -0.05f, 2.4f);
                _globalData.BigTreeTeleportToMap.transform.Rotate(new Vector3(0, 20, 0));
            }


            var orbPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(MapFileUtils).Assembly.Location), Constants.MiscObjectsFolderName, "Orb");
            AssetBundle orbBundle = MapFileUtils.GetAssetBundleFromZip(orbPath);

            GameObject orb = Object.Instantiate(orbBundle.LoadAsset<GameObject>("_Orb"));
            orb.transform.position += new Vector3(-.5f, 0.5f, -1.95f) / 4;
            GameObject orbVisuals = Object.Instantiate(orb);

            orb.AddComponent<RotateByHand>();
            orb.GetComponent<Renderer>().enabled = false;
            Object.Destroy(orb.GetComponent<Renderer>());
            orb.layer = 18;

            orbVisuals.transform.SetParent(orb.transform);
            orbVisuals.transform.localPosition = Vector3.zero;
            orbVisuals.transform.localScale = Vector3.one;
            orbVisuals.transform.localRotation = Quaternion.identity;

            orbVisuals.AddComponent<PreviewOrb>();
            orbVisuals.GetComponent<Collider>().enabled = false;
            Object.Destroy(orbVisuals.GetComponent<Collider>());

            //orb.layer = Constants.MaskLayerPlayerTrigger;

            Teleporter treeTeleporter = _globalData.BigTreeTeleportToMap.GetComponent<Teleporter>();
            treeTeleporter.TeleporterType = TeleporterType.Map;
            treeTeleporter.JoinGameOnTeleport = true;
            treeTeleporter.TeleportPoints = new List<Transform>();
            treeTeleporter.Delay = 1.5f;
            treeTeleporter.TouchType = GorillaTouchType.Head;

            Object.DontDestroyOnLoad(treeTeleporter);
            if(_globalData.BigTreePoint == null)
            {
                _globalData.BigTreePoint = new GameObject("TreeHomeTargetObject");
                _globalData.BigTreePoint.transform.position = new Vector3(-66f, 12.3f, -83f);
                _globalData.BigTreePoint.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
                Object.DontDestroyOnLoad(_globalData.BigTreePoint);
            }
            treeTeleporter.TeleportPoints.Add(_globalData.BigTreePoint.transform);

            ColorTreeTeleporter(new Color(0, 1, 0));
            HideTreeTeleporter();

            // Emergency Teleport Stuff
            if (_globalData.FallEmergencyTeleport != null)
            {
                Object.Destroy(_globalData.FallEmergencyTeleport);
                _globalData.FallEmergencyTeleport = null;
            }
            _globalData.FallEmergencyTeleport = new GameObject("FallEmergencyTeleport");
            _globalData.FallEmergencyTeleport.layer = Constants.MaskLayerHandTrigger;
            _globalData.FallEmergencyTeleport.AddComponent<BoxCollider>().isTrigger = true;
            _globalData.FallEmergencyTeleport.transform.localScale = new Vector3(1000f, 1f, 1000f);
            _globalData.FallEmergencyTeleport.transform.position = _globalData.TreeOrigin + new Vector3(0f, -200f, 0f);
            Object.DontDestroyOnLoad(_globalData.FallEmergencyTeleport);

            Teleporter emergencyFallTeleporter = _globalData.FallEmergencyTeleport.AddComponent<Teleporter>();
            emergencyFallTeleporter.TeleportPoints = new List<Transform>() { _globalData.BigTreePoint.transform };
        }
    }
}