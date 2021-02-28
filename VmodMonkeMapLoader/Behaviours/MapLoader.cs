using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using VmodMonkeMapLoader.Helpers;
using VmodMonkeMapLoader.Models;
using Zenject;
using Logger = VmodMonkeMapLoader.Helpers.Logger;
using Object = UnityEngine.Object;

namespace VmodMonkeMapLoader.Behaviours
{
    public class MapLoader : MonoBehaviour
    {
        private static GameObject _mapInstance;
        private static bool _isLoading;
        private static GlobalData _globalData;

        private void Awake()
        {
            InitializeGlobalData();
        }
        
        public void LoadMap(MapInfo mapInfo, Action<bool> isSuccess)
        {
            StartCoroutine(LoadMapFromPackageFileAsync(mapInfo, b =>
            {
                Debug.Log("______ MAP LOADED");
                isSuccess(b);
            }));
        }
        
        public IEnumerator LoadMapFromPackageFileAsync(MapInfo mapInfo, Action<bool> isSuccess)
        {
            if (_isLoading)
            {
                yield break;
            }

            _isLoading = true;

            Logger.LogText("Loading map: " + mapInfo.FilePath + "\\" + mapInfo.PackageInfo.PcFileName);

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

            _globalData.IsLegacyMap = false;

            var assetName = assetNames.FirstOrDefault(n => n.ToLowerInvariant().Contains(mapInfo.PackageInfo.Descriptor.Name.ToLowerInvariant()));

            if (string.IsNullOrWhiteSpace(assetName))
            {
                assetName = assetNames.FirstOrDefault(n => n.ToLowerInvariant().Contains(Constants.MapRootNameOrigin));
                if (string.IsNullOrWhiteSpace(assetName))
                {
                    _globalData.IsLegacyMap = true;
                    assetName = assetNames.FirstOrDefault(n => n.ToLowerInvariant().Contains(Constants.MapRootNameLegacy));
                }
                if (string.IsNullOrWhiteSpace(assetName))
                {
                    Logger.LogText("Map asset not found. Asset names in bundle: " + string.Join("|", assetNames));

                    _isLoading = false;
                    isSuccess(false);
                    bundle.Unload(false);
                    yield break;
                }
            }

            Logger.LogText("Asset name: " + assetName);

            var mapRequest = bundle.LoadAssetAsync<GameObject>(assetName);
            yield return mapRequest;

            var map = mapRequest.asset as GameObject;
            if (map == null)
            {
                _isLoading = false;
                isSuccess(false);
                bundle.Unload(false);
                yield break;
            }

            Logger.LogText("Map asset loaded: " + map.name);

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

                Destroy(_mapInstance);

                _mapInstance = null;
                Resources.UnloadUnusedAssets();
            }
        }

        private async Task ProcessAndInstantiateMap(GameObject map)
        {
            await Task.Factory.StartNew(() =>
            {
                _globalData.BigTreeTeleportToMap.GetComponent<PlayerTeleporter>().Destinations.Clear();
                _globalData.BigTreeTeleportToMap.GetComponent<PlayerTeleporter>().Destinations
                    .Add(_globalData.TeleportTargetToBigTree);
                
                ProcessChildObjects(map);
                
                InstantiateMap(map);
            });
        }

        private void ProcessChildObjects(GameObject parent)
        {
            DestroyExistingTeleports();

            Logger.LogText("Processing parent: " + parent.name + ", childs: " + parent.transform.childCount);

            for (var i = 0; i < parent.transform.childCount; i++)
            {
                var child = parent.transform.GetChild(i).gameObject;

                Logger.LogText("Processing object: " + child.name);

                FixShader(child);

                SetupCollisions(child);

                switch (child.name)
                {
                    case Constants.ObjectNameTreeTeleportTarget:
                    case Constants.ObjectNameTreeTeleportTargetLegacyStart:
                        SetTreeTeleportTargetToMap(child);
                        break;

                    case Constants.ObjectNameTreeTeleportTrigger:
                    case Constants.ObjectNameTreeTeleportTargetLegacyEnd:
                        SetTeleportTriggerBackToTree(child);
                        break;

                    default:
                        if (child.name.StartsWith(Constants.ObjectNameTeleportTarget))
                        {
                            SetupTeleportTaget(child);
                        }
                        else if (child.name.StartsWith(Constants.ObjectNameTeleportTrigger))
                        {
                            SetupTeleportTrigger(child);
                        }
                        break;
                }

                if (child.transform.childCount > 0)
                {
                    ProcessChildObjects(child);
                }
            }
        }

        private void DestroyExistingTeleports()
        {
            if (_globalData.Teleports.Any())
            {
                foreach (var teleportList in _globalData.Teleports.Values)
                {
                    foreach (var trigger in teleportList.Triggers)
                    {
                        Destroy(trigger);
                    }
                }

                _globalData.Teleports.Clear();
            }
        }

        private void FixShader(GameObject child)
        {
            var renderer = child.GetComponent<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                var shader = Shader.Find(renderer.material.shader.name);
                if (shader == null)
                {
                    shader = Shader.Find(Constants.DefaultShaderName);
                }

                renderer.material.shader = shader;
            }
        }

        private void SetupCollisions(GameObject child)
        {
            var colliders = child.GetComponents<Collider>();
            foreach (var collider in colliders)
            {
                if (collider.isTrigger)
                {
                    child.layer = Constants.MaskLayerGorillaTrigger;
                    break;
                }
            }
        }

        private void SetTreeTeleportTargetToMap(GameObject child)
        {
            _globalData.BigTreeTeleportToMap.GetComponent<PlayerTeleporter>().Destinations.Clear();
            _globalData.BigTreeTeleportToMap.GetComponent<PlayerTeleporter>().Destinations
                .Add(new TeleportTarget
                {
                    Position = _globalData.CustomOrigin + child.transform.position,
                    RotationAngle = child.transform.rotation.eulerAngles.y
                });
        }

        private void SetTeleportTriggerBackToTree(GameObject child)
        {
            if (_globalData.TeleportTriggerBackToBigTree != null)
            {
                Destroy(_globalData.TeleportTriggerBackToBigTree);
            }

            _globalData.TeleportTriggerBackToBigTree = Instantiate(_globalData.TeleportPrefab,
                _globalData.CustomOrigin + child.transform.position, child.transform.rotation);
            _globalData.TeleportTriggerBackToBigTree.GetComponent<PlayerTeleporter>().Destinations
                .Add(_globalData.TeleportTargetToBigTree);
        }

        private void SetupTeleportTaget(GameObject child)
        {
            var teleportIdIndex = child.name.IndexOf('_') + 1;
            if (teleportIdIndex > child.name.Length - 1)
            {
                Logger.LogText("Teleport ID not valid: " + child.name);
                return;
            }

            var teleportId = child.name.Substring(teleportIdIndex);

            Logger.LogText("Teleport ID: " + teleportId);

            if (string.IsNullOrWhiteSpace(teleportId))
            {
                Logger.LogText("Teleport ID not valid: " + child.name);
                return;
            }

            if (!_globalData.Teleports.ContainsKey(teleportId))
            {
                Logger.LogText("Target New info");

                _globalData.Teleports[teleportId] = new TeleportData();
            }

            var teleportInfoTarget = _globalData.Teleports[teleportId];
            teleportInfoTarget.Targets.Add(new TeleportTarget
            {
                Position = _globalData.CustomOrigin + child.transform.position,
                RotationAngle = child.transform.rotation.eulerAngles.y
            });

            if (teleportInfoTarget.Triggers.Any())
            {
                Logger.LogText("Setting destination target");

                foreach (var trigger in teleportInfoTarget.Triggers)
                {
                    trigger.GetComponent<PlayerTeleporter>().Destinations = teleportInfoTarget.Targets;
                }
            }
        }

        private void SetupTeleportTrigger(GameObject child)
        {
            var teleportIdIndex = child.name.IndexOf('_') + 1;
            if (teleportIdIndex > child.name.Length - 1)
            {
                Logger.LogText("Teleport ID not valid: " + child.name);
                return;
            }

            var teleportId = child.name.Substring(teleportIdIndex);

            Logger.LogText("Teleport ID: " + teleportId);

            if (string.IsNullOrWhiteSpace(teleportId))
            {
                Logger.LogText("Teleport ID not valid: " + child.name);
                return;
            }

            if (!_globalData.Teleports.ContainsKey(teleportId))
            {
                Logger.LogText("Trigger New info");

                _globalData.Teleports[teleportId] = new TeleportData();
            }

            var teleportInfoTrigger = _globalData.Teleports[teleportId];

            var newTrigger = Instantiate(_globalData.TeleportPrefab,
                _globalData.CustomOrigin + child.transform.position, child.transform.rotation);
            newTrigger.GetComponent<Renderer>().material.color = Color.red;
            newTrigger.GetComponent<PlayerTeleporter>().Destinations = teleportInfoTrigger.Targets;

            teleportInfoTrigger.Triggers.Add(newTrigger);
        }

        private void InstantiateMap(GameObject map)
        {
            UnloadMap();

            Logger.LogText("Instantiate map");

            _mapInstance = Instantiate(map, _globalData.CustomOrigin + (_globalData.IsLegacyMap ? Constants.LegacyMapOffset : Vector3.zero), Quaternion.identity);
        }

        private void InitializeGlobalData()
        {
            if(_globalData == null)
                _globalData = new GlobalData();

            _globalData.Origin = GameObject.Find("slide")?.transform.position ?? Vector3.zero;

            _globalData.TreeOrigin = GameObject.Find("stool")?.transform.position ?? Vector3.zero;

            if (_globalData.BigTreeTeleportToMap != null)
            {
                Destroy(_globalData.BigTreeTeleportToMap);
                _globalData.BigTreeTeleportToMap = null;
            }
            _globalData.BigTreeTeleportToMap = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _globalData.BigTreeTeleportToMap.layer = Constants.MaskLayerHandTrigger;
            _globalData.BigTreeTeleportToMap.GetComponent<Collider>().isTrigger = true;
            _globalData.BigTreeTeleportToMap.GetComponent<Renderer>().material.color = Color.green;
            _globalData.BigTreeTeleportToMap.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            _globalData.BigTreeTeleportToMap.transform.position =
                _globalData.TreeOrigin + new Vector3(0f, 0.5f, -1f);
            _globalData.BigTreeTeleportToMap.AddComponent<PlayerTeleporter>().Destinations
                .Add(_globalData.TeleportTargetToBigTree);

            if (_globalData.FallEmergencyTeleport != null)
            {
                Destroy(_globalData.FallEmergencyTeleport);
                _globalData.FallEmergencyTeleport = null;
            }
            _globalData.FallEmergencyTeleport = new GameObject("FallEmergencyTeleport");
            _globalData.FallEmergencyTeleport.layer = Constants.MaskLayerHandTrigger;
            _globalData.FallEmergencyTeleport.AddComponent<BoxCollider>().isTrigger = true;
            _globalData.FallEmergencyTeleport.transform.localScale = new Vector3(1000f, 1f, 1000f);
            _globalData.FallEmergencyTeleport.transform.position = _globalData.TreeOrigin + new Vector3(0f, -200f, 0f);
            _globalData.FallEmergencyTeleport.AddComponent<PlayerTeleporter>().Destinations
                .Add(_globalData.TeleportTargetToBigTree);

            if (_globalData.TeleportPrefab != null)
            {
                Destroy(_globalData.TeleportPrefab);
                _globalData.TeleportPrefab = null;
            }
            _globalData.TeleportPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _globalData.TeleportPrefab.layer = Constants.MaskLayerHandTrigger;
            _globalData.TeleportPrefab.GetComponent<Collider>().isTrigger = true;
            _globalData.TeleportPrefab.GetComponent<Renderer>().material.color = Color.green;
            _globalData.TeleportPrefab.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            _globalData.TeleportPrefab.transform.position = new Vector3(0f, -1000f, 0f);
            _globalData.TeleportPrefab.AddComponent<PlayerTeleporter>().Destinations
                .Add(_globalData.TeleportTargetToBigTree);
        }
    }
}