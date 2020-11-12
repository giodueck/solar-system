using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float mass = 10;
    public Vector3 velocity;
    public Universe universe;
    protected Rigidbody rb = null;

    void Start()
    {
        GenerateBody();
        SetRigidbody(GetComponent<Rigidbody>());
    }

    public void SetRigidbody(Rigidbody rigidbody)
    {
        rb = rigidbody;
        if (rb != null)
        {
            rb.mass = mass;
            if (mass == 0)
            {
                mass = (float) 1e-07;
            }
            rb.useGravity = false;
            rb.angularDrag = 0;
        }
    }

    public virtual void GenerateBody()
    {
        Debug.Log(this.name + " does not have a GenerateBody() method.");
    }
}
