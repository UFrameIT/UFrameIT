using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CircleGenerator : ShapeGenerator
{
    #region InspectorVariables
    [Header("Circle values")]
    [Range(0,100)] public Vector3 midPoint = Vector3.zero;
    [Range(0,100)] public float radius = 1f;

    [Header("Technical")]
    [Range(3,1000)] public int sideCount = 500;

    [Header("Parts")]
    public MeshFilter circleMesh;
    #endregion InspectorVariables

    #region Implementation
    protected override void GenerateShape()
    {
        var circle = CreateCircle(radius, sideCount);

        if (circleMesh.sharedMesh != null)
            circleMesh.sharedMesh.Clear();
        circleMesh.mesh = CreateMesh(circle);

        if (circleMesh.transform.TryGetComponent(out MeshCollider meshCol))
            meshCol.sharedMesh = circleMesh.sharedMesh;
    }

    /// <summary>
    /// Creates circle vertecies and triangles around the midPoint at (0,0,0)
    /// </summary>
    /// <param name="points"></param>
    /// <param name="invert"></param>
    /// <returns></returns>
    static (Vector3[], int[]) CreateCircle(float radius, int sideCount, bool invert = false)
    {
        Vector3[] vertices = GetCirclePoints(radius, sideCount).Union(new Vector3[] { Vector3.zero }).ToArray();
        int[] triangles = new int[(vertices.Length - 1) * 3];
        int vertLen = vertices.Length;
        for (int i = 0; i < vertLen-1; i++)
        {
            triangles[i * 3 + 0] = vertLen-1; // midPoint
            triangles[i * 3 + 1] = i;
            triangles[i * 3 + 2] = (i + 1) % (vertLen-1);
        }
        return (vertices, invert ? triangles.Reverse().ToArray() : triangles);
    }
    #endregion Implementation
}
