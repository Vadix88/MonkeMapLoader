using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using VmodMonkeMapLoader.Models;

namespace VmodMonkeMapLoader.Behaviours
{
    public class PreviewOrb : MonoBehaviour
    {
        private static PreviewOrb _instance;

        void Start()
        {
            _instance = this;
            gameObject.GetComponent<Renderer>().enabled = false;
        }

        public static void ChangeOrb(Color color, Texture2D texture)
        {
            Renderer orbRenderer = _instance.gameObject.GetComponent<Renderer>();
            orbRenderer.enabled = true;
            orbRenderer.sharedMaterials[0].SetColor("_Color", color);
            orbRenderer.sharedMaterials[1].SetTexture("_MainTex", texture);
        }
        public static void ChangeOrb(MapInfo mapInfo) => ChangeOrb(mapInfo.PackageInfo.Config.MapColor, mapInfo.PackageInfo.PreviewCubemap);
    }
}
