using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TorusGenerator : ShapeGenerator
{
    #region InspectorVariables
    [Header("Torus values")]
    [Range(0, 100)] public float torusRadius = 1f;
    [Range(0, 10)]  public float ringRadius = 0.1f;

    [Header("Technical")]
    [Range(8, 150)] public int ringSegmentCount = 50;
    [Range(3, 100)] public int segmentSideCount = 30;

    [Header("Parts")]
    public MeshFilter torusMesh;
    #endregion InspectorVariables

    #region Implementation
    protected override void GenerateShape()
    {
        if (torusMesh.sharedMesh != null)
            torusMesh.sharedMesh.Clear();
        torusMesh.mesh = CreateMesh(CreateTorus(torusRadius, ringRadius, ringSegmentCount, segmentSideCount));

        if (torusMesh.transform.TryGetComponent(out MeshCollider meshCol))
            meshCol.sharedMesh = torusMesh.sharedMesh;
    }

    private static (Vector3[] vertices, int[] triangles) CreateTorus(float torusRadius, float ringRadius, int ringSegmentCount, int segmentSideCount)
    {
        Vector3[] vertices = new Vector3[ringSegmentCount * segmentSideCount * 4];
        int[] triangles = new int[ringSegmentCount * segmentSideCount * 6];

        //generate vertices
        float iStep = (2f * Mathf.PI) / ringSegmentCount;
        float jStep = (2f * Mathf.PI) / segmentSideCount;
        for (int i = 0; i < ringSegmentCount; i++)
        {
            for (int j = 0; j < segmentSideCount; j++)
            {
                vertices[(i * segmentSideCount + j) * 4]     = GetPointOnTorus(torusRadius, ringRadius, i * iStep, j * jStep);
                vertices[(i * segmentSideCount + j) * 4 + 1] = GetPointOnTorus(torusRadius, ringRadius, i * iStep, (j + 1) * jStep);
                vertices[(i * segmentSideCount + j) * 4 + 2] = GetPointOnTorus(torusRadius, ringRadius, (i + 1) * iStep, j * jStep);
                vertices[(i * segmentSideCount + j) * 4 + 3] = GetPointOnTorus(torusRadius, ringRadius, (i + 1) * iStep, (j + 1) * jStep);
            }
        }

        //generate triangles
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t]   = i;
            triangles[t+1] = i + 1;
            triangles[t+2] = i + 2;

            triangles[t+3] = i + 1;
            triangles[t+4] = i + 3;
            triangles[t+5] = i + 2;
        }

        return (vertices, triangles);
    }

    private static Vector3 GetPointOnTorus(float torusRadius, float ringRadius, float u, float v)
    {
        float r = (torusRadius + ringRadius * Mathf.Sin(v));
        return new Vector3(r * Mathf.Sin(u), ringRadius * Mathf.Cos(v), r * Mathf.Cos(u));
    }
    #endregion Implementation
}