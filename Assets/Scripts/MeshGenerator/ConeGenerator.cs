using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ConeGenerator : ShapeGenerator
{
    #region InspectorVariables
    [Header("Cone values")]
    [Range(0,100)] public float bottomRadius = 1f;
    [Range(0,100)] public float topRadius = 0f;
    public Vector3 topPosition = new Vector3(0, 1f, 0);

    [Header("Technical")]
    [Range(3,1000)] public int sideCount = 500;
    public bool generateTop = true;
    public bool generateBottom = false;

    [Header("Parts")]
    public MeshFilter topMesh;
    public MeshFilter sideMesh;
    public MeshFilter bottomMesh;
    #endregion InspectorVariables

    #region Implementation
    protected override void GenerateShape()
    {
        Vector3[] bottomCircle = GetCirclePoints(bottomRadius, sideCount);
        Vector3[] topCircle = GetCirclePoints(topRadius, sideCount, topPosition);

        //side
        if (sideMesh.sharedMesh != null)
            sideMesh.sharedMesh.Clear();
        sideMesh.mesh = CreateMesh(CreateConeSide(sideCount, bottomCircle, topCircle));

        //top
        if (topMesh.sharedMesh != null)
            topMesh.sharedMesh.Clear();
        if (generateTop)
            topMesh.mesh = CreateMesh(CreatePlane(topCircle, false));

        //bottom
        if (bottomMesh.sharedMesh != null)
            bottomMesh.sharedMesh.Clear();
        if (generateBottom)
            bottomMesh.mesh = CreateMesh(CreatePlane(bottomCircle, true));
    }

    private static (Vector3[], int[]) CreateConeSide(int sideCount, Vector3[] bottomCircle, Vector3[] topCircle)
    {
        Vector3[] vertices = new Vector3[sideCount * 4];
        int[] triangles = new int[sideCount * 6];

        //generate vertices
        for (int i = 0; i < sideCount; i++)
        {
            vertices[i * 4] = bottomCircle[i];
            vertices[i * 4 + 1] = bottomCircle[(i + 1) % sideCount];
            vertices[i * 4 + 2] = topCircle[i];
            vertices[i * 4 + 3] = topCircle[(i + 1) % sideCount];
        }

        //generate triangles
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t]     = i;
            triangles[t + 1] = i + 1;
            triangles[t + 2] = i + 2;

            triangles[t + 3] = i + 1;
            triangles[t + 4] = i + 3;
            triangles[t + 5] = i + 2;
        }

        return (vertices, triangles);
    }
    #endregion Implementation
}
