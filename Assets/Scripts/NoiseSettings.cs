using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Ridgid }
    public FilterType filterType;

    [ConditionalHide("filterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)]
    public RidgidNoiseSettings ridgidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float strength = 1;      // how noticeable the noise's effects are
        [Range(1, 8)]
        public int numLayers = 1;       // more layers => more detail
        public float baseRoughness = 1; // roughness of first layer
        public float roughness = 2;     // how the frequency changes: more roughness = more detail
        public float persistence = .5f; // how the amplitud changes: less persistence = less amplitude
        public Vector3 center;          // moves the point at which the noise is evaluated
        public float minValue = 0;      // height of the noise, if less than the radius, it does not affect the surface
    }

    [System.Serializable]
    public class RidgidNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultiplier = .8f;    // how much the weight grows
    }
}
