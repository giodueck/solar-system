using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RidgidNoiseFilter : INoiseFilter
{
    NoiseSettings.RidgidNoiseSettings settings;
    Noise noise = new Noise();

    public RidgidNoiseFilter(NoiseSettings.RidgidNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;

        // Noise will be layered if numLayers > 1, with increasing frequency and decreasing amplitude
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.center));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier);
            noiseValue += v * amplitude;

            // If roughness > 1, frequency of noise will increase with each iteration
            // if persistence < 1, ampplitude of noise will decrease with each iteration
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        // if noiseValue < minValue, it will not affect the surface
        noiseValue = noiseValue - settings.minValue;
        return noiseValue * settings.strength;
    }
}
