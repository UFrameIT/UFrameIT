using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CircleSegmentGenerator : MonoBehaviour
{
    public float radius;
    public float height;

    private Mesh mesh;

    //Every 0.5° of the circle segment, there starts a new triangle
    private float angleAccuracy = 0.5f;

    public void setAngle(float angle)
    {
        CreateSegment(angle, radius);
    }

    private void CreateSegment(float angle, float radius)
    {
        float absoluteAngle = Mathf.Abs(angle);
        List<Vector3> verticeList = new List<Vector3>();
        List<int> triangleList = new List<int>();

        //Center-Point of lower side
        Vector3 center0 = new Vector3(0, 0, 0);
        int center0Index = 0;
        verticeList.Add(center0);

        //Center-Point of upper side
        Vector3 center1 = new Vector3(0, height, 0);
        int center1Index = 1;
        verticeList.Add(center1);

        float posAngle = absoluteAngle / 2;
        float negAngle = posAngle * -1;

        int i = 2;
        for (float x = negAngle; x < posAngle; x += angleAccuracy)
        {
            float nextAngle;

            if (x + angleAccuracy > posAngle)
                nextAngle = posAngle;
            else
                nextAngle = x + angleAccuracy;

            float newPointX = radius * Mathf.Cos(nextAngle * Mathf.Deg2Rad);
            float newPointZ = radius * Mathf.Sin(nextAngle * Mathf.Deg2Rad);


            if (i == 2)
            {
                //Add first Points at the beginning of the angle
                float firstPointX = radius * Mathf.Cos(negAngle * Mathf.Deg2Rad);
                float firstPointZ = radius * Mathf.Sin(negAngle * Mathf.Deg2Rad);
                verticeList.Add(new Vector3(firstPointX, 0, firstPointZ));
                verticeList.Add(new Vector3(firstPointX, height, firstPointZ));
                
                //Adding triangles for left side
                triangleList.Add(center0Index);
                triangleList.Add(center1Index);
                triangleList.Add(i + 1);
                triangleList.Add(center0Index);
                triangleList.Add(i + 1);
                triangleList.Add(i);

                i += 2;
            }

            verticeList.Add(new Vector3(newPointX, 0, newPointZ));
            verticeList.Add(new Vector3(newPointX, height, newPointZ));

            //Adding triangles for upper- and lower-side
            triangleList.Add(center0Index);
            triangleList.Add(i - 2);
            triangleList.Add(i);
            triangleList.Add(center1Index);
            triangleList.Add(i + 1);
            triangleList.Add(i - 1);
            //Adding triangles for front side
            triangleList.Add(i - 2);
            triangleList.Add(i - 1);
            triangleList.Add(i + 1);
            triangleList.Add(i - 2);
            triangleList.Add(i + 1);
            triangleList.Add(i);

            if (nextAngle == posAngle)
            {
                //Adding triangles for right side
                triangleList.Add(center0Index);
                triangleList.Add(i + 1);
                triangleList.Add(center1Index);
                triangleList.Add(center0Index);
                triangleList.Add(i);
                triangleList.Add(i + 1);
            }

            i += 2;
        }

        mesh = new Mesh();
        mesh.vertices = verticeList.ToArray();
        mesh.triangles = triangleList.ToArray();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.RecalculateNormals();
    }
}
