using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {

    Mesh mesh;
    int res;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(Mesh mesh, int res, Vector3 localUp)
    {
        this.mesh = mesh;
        this.res = res;
        this.localUp = localUp;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[res * res];
        int[] triangles = new int[(res - 1) * (res - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                int i = x + y * res;
                Vector2 percent = new Vector2(x, y) / (res - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;

                vertices[i] = pointOnUnitSphere;
                if (x != res - 1 && y != res - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + res + 1;
                    triangles[triIndex + 2] = i + res;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + res + 1;
                    triIndex += 6;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

}
