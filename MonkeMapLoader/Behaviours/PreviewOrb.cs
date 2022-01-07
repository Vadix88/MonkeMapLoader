using UnityEngine;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.Behaviours
{
    public class PreviewOrb : MonoBehaviour
    {
        private static PreviewOrb _instance;

        private Renderer orbRenderer;

        void Start()
        {
            _instance = this;
            orbRenderer = gameObject.GetComponent<Renderer>();
            orbRenderer.enabled = false;
        }

        public static void ChangeOrb(Color color, Texture2D texture)
        {
            _instance.orbRenderer.enabled = true;
            _instance.orbRenderer.sharedMaterials[0].SetColor("_Color", color);
            _instance.orbRenderer.sharedMaterials[1].SetTexture("_MainTex", texture);
        }
        public static void ChangeOrb(MapInfo mapInfo) => ChangeOrb(mapInfo.PackageInfo.Config.MapColor, mapInfo.PackageInfo.PreviewCubemap);

        public static void HideOrb()
		{
            _instance.orbRenderer.enabled = false;
		}
    }
}
