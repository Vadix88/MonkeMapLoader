using System.Net.Http;
using UnityEngine;
using VmodMonkeMapLoader.Models;
using Zenject;

namespace VmodMonkeMapLoader.Behaviours
{
    public class PreviewOrb : MonoBehaviour
    {
        private static PreviewOrb _instance;

        private HttpClient client;

        private Renderer orbRenderer;

		void Start()
        {
            _instance = this;
            orbRenderer = gameObject.GetComponent<Renderer>();
            orbRenderer.enabled = false;

            client = new HttpClient();
        }

        public static void ChangeOrb(Color color, Texture2D texture)
        {
            _instance.orbRenderer.enabled = true;
            _instance.orbRenderer.sharedMaterials[0].SetColor("_Color", color);
            _instance.orbRenderer.sharedMaterials[1].SetTexture("_MainTex", texture);
        }
        public static void ChangeOrb(MapInfo mapInfo) => ChangeOrb(mapInfo.PackageInfo.Config.MapColor, mapInfo.PackageInfo.PreviewCubemap);

		public static void ChangeOrb(MonkeMapHubResponse.Map selectedMap)
		{
			string url = Helpers.Constants.MonkeMapHubBase + selectedMap.MapThumbnailFileUrl.Replace("preview", "preview_cubemap");
			ChangeOrb(url);
		}

		public static async void ChangeOrb(string url)
		{
			var response = await _instance.client.GetAsync(url);

			if (response.IsSuccessStatusCode)
			{
				var texture = new Texture2D(1, 1);
				texture.LoadImage(await response.Content.ReadAsByteArrayAsync());
				// Get average color of image
				int count = 0;
				Color average = new Color(0, 0, 0, 0);
				foreach (var pixel in texture.GetPixels())
				{
					average += pixel;
					count++;
				}

				average /= count;

				// TODO: make this the correct color
				ChangeOrb(average, texture);
			}
		}

		public static void HideOrb()
		{
            _instance.orbRenderer.enabled = false;
		}
	}
}
