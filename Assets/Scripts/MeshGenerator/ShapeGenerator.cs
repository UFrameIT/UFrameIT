using System.Linq;
using UnityEngine;

public abstract class ShapeGenerator : MonoBehaviour
{
    #region UnityMethods
    void Start() => GenerateShape();

    void OnValidate() => GenerateShape();
    #endregion UnityMethods

    protected abstract void GenerateShape();

    #region Helper
    protected static Mesh CreateMesh((Vector3[] vertices, int[] triangles) meshValues)
    {
        Mesh mesh = new Mesh();
        (mesh.vertices, mesh.triangles) = (meshValues.vertices, meshValues.triangles);
        mesh.RecalculateNormals(); //fix lighting
        return mesh;
    }

    protected static Vector3[] GetCirclePoints(float circleRadius, int pointCount) => GetCirclePoints(circleRadius, pointCount, Vector3.zero);
    protected static Vector3[] GetCirclePoints(float circleRadius, int pointCount, Vector3 offset)
    {
        Vector3[] circle = new Vector3[pointCount];
        float slice = (2f * Mathf.PI) / pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * slice;
            circle[i] = new Vector3(circleRadius * Mathf.Sin(angle), 0, circleRadius * Mathf.Cos(angle)) + offset;
        }
        return circle;
    }

    /// <summary>
    /// Creates triangles for a set of vertecies of a flat, convex shape
    /// </summary>
    /// <param name="points"></param>
    /// <param name="invert"></param>
    /// <returns></returns>
    protected static (Vector3[], int[]) CreatePlane(Vector3[] points, bool invert = false)
    {
        Vector3[] vertices = points;
        int[] triangles = new int[(vertices.Length - 2) * 3];
        for (int i = 1; i < vertices.Length - 1; i++)
        {
            triangles[(i - 1) * 3 + 0] = 0;
            triangles[(i - 1) * 3 + 1] = i;
            triangles[(i - 1) * 3 + 2] = (i + 1);
        }
        return (vertices, invert ? triangles.Reverse().ToArray() : triangles);
    }
    #endregion Helper
}
