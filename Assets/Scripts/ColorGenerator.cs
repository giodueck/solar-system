using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator
{
    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;

    // Replaces the constructor so a new object and texture don't have to be created on update
    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if (texture == null || texture.height != settings.biomeColorSettings.biomes.Length || (texture.width / textureResolution == 1 && settings.hasOcean) || (texture.width / textureResolution == 2 && !settings.hasOcean))
        {
            int width = 1 + ((settings.hasOcean) ? 1 : 0);
            texture = new Texture2D(textureResolution * 2, settings.biomeColorSettings.biomes.Length, TextureFormat.RGBA32, false);
        }
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    // For use in ShaderGraph
    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.bodyMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    // Returns a float from 0 to 1 depending on which biome the point is in
    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.
            biomeColorSettings.noiseStrength;
        float biomeIndex = 0;
        int numBiomes = settings.biomeColorSettings.biomes.Length;
        float blendRange = settings.biomeColorSettings.blendAmount / 2f + 0.001f;

        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    // For use in ShaderGraph, sets the colors from the gradient(s) in settings
    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];
        int colorIndex = 0;
        foreach (var biome in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++, colorIndex++)
            {
                Color gradientColor;
                if (settings.hasOcean)
                {
                    if (i < textureResolution)
                    {
                        gradientColor = settings.oceanColor.Evaluate(i / (textureResolution - 1f));
                    }
                    else
                    {
                        gradientColor = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                    }
                }
                else
                {
                    gradientColor = biome.gradient.Evaluate(i / (textureResolution - 1f));
                }

                Color tintColor = biome.tint;
                colors[colorIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
            }
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.bodyMaterial.SetTexture("_texture", texture);
    }
}
