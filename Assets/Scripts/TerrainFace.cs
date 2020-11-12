using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace
{
    Mesh mesh;
    int resolution;
    ShapeGenerator shapeGenerator;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        // Create two aditional vectors parallel to localUp that are the local x and z axes
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    // Constructs the mesh for this TerrainFace object from scratch
    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        Vector2[] uv = (mesh.uv.Length == vertices.Length) ? mesh.uv : new Vector2[vertices.Length];

        for (int y = 0, i = 0, triIndex = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++, i++)
            {
                // assign vertices
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                float unscaledElevation = shapeGenerator.CalculateUnscaledElevation(pointOnUnitSphere);
                vertices[i] = pointOnUnitSphere * shapeGenerator.GetScaledElevation(unscaledElevation);
                uv[i].y = unscaledElevation;

                // define triangles using indices to vertices
                if (x != resolution - 1 && y != resolution - 1)
                {
                    // triangles are stored in groups of 3 vertices
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6;
                }
            }
        }
        // Assign new mesh vertices and triangles
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.uv = uv;
    }

    // UpdateUVs is outside of GenerateMesh so that updating the colors doesn't also regenerate
    // the meshes, since that can be slow
    public void UpdateUVs(ColorGenerator colorGenerator)
    {
        Vector2[] uv = mesh.uv;

        for (int y = 0, i = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++, i++)
            {
                // assign vertices
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                uv[i].x = colorGenerator.BiomePercentFromPoint(pointOnUnitSphere);
            }
        }
        mesh.uv = uv;
    }
}
