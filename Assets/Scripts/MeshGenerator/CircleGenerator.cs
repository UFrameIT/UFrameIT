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
        Vector3[] circle = GetCirclePoints(radius, sideCount, Vector3.zero);

        if (circleMesh.sharedMesh != null)
            circleMesh.sharedMesh.Clear();
        circleMesh.mesh = CreateMesh(CreatePlane(circle));
    }
    #endregion Implementation
}
