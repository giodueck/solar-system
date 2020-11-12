using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleNoiseFilter : INoiseFilter
{
    NoiseSettings.SimpleNoiseSettings settings;
    Noise noise = new Noise();

    public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings)
    {
        this.settings = settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        // Noise will be layered if numLayers > 1, with increasing frequency and decreasing amplitude
        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.center);
            noiseValue += (v + 1) * .5f * amplitude;

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
