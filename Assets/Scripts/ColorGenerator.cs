using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGenerator {

    ColorSettings settings;
    Texture2D texture;
    const int textureResolution = 50;
    INoiseFilter biomeNoiseFilter;

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if (texture == null || texture.height != settings.biomeColorSettings.biomes.Length) texture = new Texture2D(textureResolution * 2, settings.biomeColorSettings.biomes.Length, TextureFormat.RGBA32, false);
        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMat.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float biomeIndex = 0;
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        int numBiomes = settings.biomeColorSettings.biomes.Length;
        float blendRange = settings.biomeColorSettings.blendAmount / 2.0f + 0.001f;

        heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);

            biomeIndex *= (1 - weight);
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColors()
    {
        int colorIndex = 0;
        Color[] colors = new Color[texture.width * texture.height];

        foreach(var biome in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                Color gradCol;
                if (i < textureResolution)
                {
                    gradCol = settings.oceanColor.Evaluate(i / (textureResolution - 1.0f));
                }
                else
                {
                    gradCol = biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1.0f));
                }
                Color tintCol = biome.tint;
                colors[colorIndex++] = gradCol * (1 - biome.tintPercent) + tintCol * biome.tintPercent;
            }
        }
        texture.SetPixels(colors);
        texture.Apply();
        settings.planetMat.SetTexture("_texture", texture);
    }

}
