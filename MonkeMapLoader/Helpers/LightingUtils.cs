using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace VmodMonkeMapLoader.Helpers
{
    public static class LightingUtils
    {
        public static void SetLightingStrength(Material material, float strength)
        {
            // Strength = 1 means max lighting strength - unchanged from export
            // Strength = 0 means no lighting (probably darkness)
            if (material == null) return;
            if (!material.HasProperty("_OcclusionMap")) return; // Non-standard shaders can't use occlusion maps
            Texture map = material.GetTexture("_OcclusionMap"); // Get the occlusion map to check if it exists already
            if (map != null && map.name.Contains("adjustedByLoader")) return; // No need to change if it's adjusted
            else if (map != null) SetExistingLightingStrength(material, strength); // IF it's an occlusion map set by the map creator then more advanced logic is needed
            else
            {
                Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                texture.name = material.name + "_OCC_adjustedByLoader";

                texture.SetPixel(0, 0, new Color(strength, strength, strength, 1));
                texture.Apply();

                material.SetTexture("_OcclusionMap", texture);
            }
        }

        public static void SetExistingLightingStrength(Material material, float strength)
        {
            // More advanced lighting logic that takes each pixel as a base and interpolates strength
            if (material == null) return;
            if (!material.HasProperty("_OcclusionMap")) return; // Non-standard shaders can't use occlusion maps

            Texture map = material.GetTexture("_OcclusionMap"); // Get the occlusion map 
            Texture2D duplicatedTexture = new Texture2D(map.width, map.height);
            duplicatedTexture.name = map.name + "_adjustedByLoader";
            Graphics.CopyTexture(map, duplicatedTexture);
            Color[] pixels = duplicatedTexture.GetPixels();

            // Iterate through each pixel and set them.
            for (int i = 0; i < pixels.Length; i++)
            {
                Color pixel = pixels[i];

                Color updatedPixel = new Color(
                    StrengthFromExistingColorFloat(pixel.r, strength),
                    StrengthFromExistingColorFloat(pixel.g, strength),
                    StrengthFromExistingColorFloat(pixel.b, strength),
                    1
                );
                pixels[i] = updatedPixel;
            }

            duplicatedTexture.SetPixels(pixels);
            duplicatedTexture.Apply();
            material.SetTexture("_OcclusionMap", duplicatedTexture);
        }

        public static float StrengthFromExistingColorFloat(float colorValue, float strength)
        {
            // Color - (Color * (1 - Strength))
            float manipulatedValue = colorValue - (colorValue * (1 - strength));
            if (manipulatedValue < 0) manipulatedValue = 0;
            return manipulatedValue;
        }
    }
}
