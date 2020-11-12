using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class UniverseSettings : ScriptableObject
{
    public float gravitationalConstant = 1f;
    public List<Planet> allBodies;
}
