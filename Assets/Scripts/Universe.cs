using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Universe : MonoBehaviour
{
    public float gravitationalConstant = 1;
    public float speedMultiplier = 1;
    public List<Planet> allBodies;

    private void Start()
    {
        foreach (var body in allBodies)
        {
            Debug.Log(body.name);
            body.GeneratePlanet();
        }
    }
    
    public void FixedUpdate()
    {
        foreach (var body in allBodies)
        {
            UpdateVelocity(body, Time.fixedDeltaTime * speedMultiplier);
        }

        foreach (var body in allBodies)
        {
            UpdatePosition(body, Time.fixedDeltaTime * speedMultiplier);
        }
    }

    public void UpdateVelocity(Planet planet, float timeStep)
    {
        foreach (var otherBody in allBodies)
        {
            if (otherBody != planet)
            {
                float sqrDst = ((otherBody.transform.position) - transform.position).sqrMagnitude;
                Vector3 forceDir = (otherBody.transform.position - transform.position).normalized;
                Vector3 force = forceDir * (gravitationalConstant * otherBody.physicsSettings.mass * planet.physicsSettings.mass) / sqrDst;
                Vector3 acceleration = force / planet.physicsSettings.mass;
                planet.physicsSettings.velocity += acceleration * timeStep;
            }
        }
    }
    
    public void UpdatePosition(Planet planet, float timeStep)
    {
        planet.transform.Translate(planet.physicsSettings.velocity * timeStep);
    }
}
