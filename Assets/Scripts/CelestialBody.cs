using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class CelestialBody : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 96;
    public bool autoUpdate = true;
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back }
    public FaceRenderMask faceRenderMask;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    // Physics
    Vector3 velocity;
    Rigidbody rb;

    // Visuals
    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;
    public PhysicsSettings physicsSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;
    [HideInInspector]
    public bool physicsSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    private void Start()
    {
        GenerateBody();
    }

    // Creates new TerrainFace objects to regenerate the mesh(es) according to new settings
    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObject = new GameObject("mesh");
                meshObject.transform.Translate(transform.position);
                meshObject.transform.parent = transform;

                meshObject.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.bodyMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);
        }
    }

    // Generates the entire celestial body
    public void GenerateBody()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
        GenerateProperties();
    }

    // Only generates shape
    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    // Only generates colors
    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    // Only updates physical properties
    public void OnPhysiscsSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateProperties();
        }
    }

    // Calls ConstructMesh on all faces
    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    // Generates the color for the different terrainfaces
    void GenerateColors()
    {
        colorGenerator.UpdateColors();

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colorGenerator);
            }
        }
    }

    // Sets physical properties of the body
    void GenerateProperties()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.angularDrag = 0;
            rb.mass = physicsSettings.surfaceGravity * shapeSettings.radius * shapeSettings.radius / Universe.gravitationalConstant;
        }
        rb.mass = physicsSettings.surfaceGravity * shapeSettings.radius * shapeSettings.radius / Universe.gravitationalConstant;
        velocity = physicsSettings.initialVelocity;
    }

    // Updates velocity by looping over all other bodies
    public void UpdateVelocity(CelestialBody[] allBodies, float timeStep)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (var otherBody in allBodies)
        {
            if (otherBody != this)
            {
                float sqrDst = (otherBody.rb.position - rb.position).sqrMagnitude;
                Vector3 forceDir = (otherBody.rb.position - rb.position).normalized;
                acceleration += forceDir * Universe.gravitationalConstant * otherBody.Mass / sqrDst;
            }
        }

        velocity += acceleration * timeStep;
    }

    // Updates velocity by just taking in an acceleration value
    public void UpdateVelocity(Vector3 acceleration, float timeStep)
    {
        velocity += acceleration * timeStep;
    }

    // Moves the rb according to current position and velocity
    public void UpdatePosition(float timeStep)
    {
        rb.MovePosition(rb.position + velocity * timeStep);
    }

    public float Mass
    {
        get
        {
            return rb.mass;
        }
    }

    public Vector3 Position
    {
        get
        {
            return rb.position;
        }
    }
}
