using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodySimulation : MonoBehaviour
{
    CelestialBody[] bodies;
    static BodySimulation instance;

    void Awake()
    {
        bodies = FindObjectsOfType<CelestialBody>();
        Time.fixedDeltaTime = Universe.physicsTimeStep;
    }

    void FixedUpdate()
    {
        // Update velocities
        for (int i = 0; i < bodies.Length; i++)
        {
            Vector3 acceleration = CalculateAcceleration(bodies[i].Position, bodies[i]);
            bodies[i].UpdateVelocity(acceleration, Universe.physicsTimeStep);
        }

        // Update positions
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdatePosition(Universe.physicsTimeStep);
        }
    }

    public static Vector3 CalculateAcceleration(Vector3 point, CelestialBody ignoreBody)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (var body in Bodies)
        {
            if (body != ignoreBody)
            {
                float sqrDst = (body.Position - point).sqrMagnitude;
                Vector3 forceDir = (body.Position - point).normalized;
                acceleration += forceDir * Universe.gravitationalConstant * body.Mass / sqrDst;
            }
        }

        return acceleration;
    }

    public static CelestialBody[] Bodies
    {
        get
        {
            return Instance.bodies;
        }
    }

    static BodySimulation Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<BodySimulation>();
            }
            return instance;
        }
    }
}
