using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PhysicsSettings : ScriptableObject
{
    public float surfaceGravity = 10;
    public Vector3 initialVelocity;
}
