using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CommunicationEvents;

public class FactSpawner : MonoBehaviour
{
    private GameObject FactRepresentation;

    void Start()
    {
       
        AddFactEvent.AddListener(FactAction);
        RemoveFactEvent.AddListener(DeleteObject);

        AnimateNonExistingFactEvent.AddListener(animateNonExistingFactTrigger);

        //Default FactRepresenation = Sphere-Prefab for Points
        this.FactRepresentation = (GameObject) Resources.Load("Prefabs/Sphere", typeof(GameObject));

    }

    public void FactAction(Fact fact) {
        getAction(fact)?.Invoke(fact);
    }

    public Func<Fact, Fact> getAction(Fact fact)
    {
        switch  (fact)
        {
            case PointFact pointFact:
                return SpawnPoint;
            case LineFact lineFact:
                return SpawnLine;
            case AngleFact angleFact:
                return SpawnAngle;
            case RayFact rayFact:
                return SpawnRay;
            default:
                return null;
        }
    }
  

    public Fact SpawnPoint(Fact pointFact)
    {
        PointFact fact = ((PointFact)pointFact);
        this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Sphere", typeof(GameObject));
     
        GameObject point = GameObject.Instantiate(FactRepresentation);
        point.transform.position = fact.Point;
        point.transform.up = fact.Normal;
        string letter = ((Char)(64+fact.Id+1)).ToString();
        point.GetComponentInChildren<TextMeshPro>().text = letter;
        point.GetComponent<FactObject>().Id = fact.Id;
        fact.Representation = point;
        return fact;
    }

    public Fact SpawnLine(Fact fact)
    {
        LineFact lineFact = ((LineFact)fact);

        PointFact pointFact1 = (Facts[lineFact.Pid1] as PointFact);
        PointFact pointFact2 = (Facts[lineFact.Pid2] as PointFact);
        Vector3 point1 = pointFact1.Point;
        Vector3 point2 = pointFact2.Point;
        //Change FactRepresentation to Line
        this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Line", typeof(GameObject));
        GameObject line = GameObject.Instantiate(FactRepresentation);
        //Place the Line in the centre of the two points
        line.transform.position = Vector3.Lerp(point1, point2, 0.5f);
        //Change scale and rotation, so that the two points are connected by the line
        //Get the Line-GameObject as the first Child of the Line-Prefab -> That's the Collider
        var v3T = line.transform.GetChild(0).localScale;
        v3T.x = (point2 - point1).magnitude;
        //For every Coordinate x,y,z we have to devide it by the LocalScale of the Child,
        //because actually the Child should be of this length and not the parent, which is only the Collider
        v3T.x = v3T.x / line.transform.GetChild(0).GetChild(0).localScale.x;
        //y and z of the line/Cube-GameObject here hard coded = ratio of sphere-prefab
        v3T.y = 0.1f / line.transform.GetChild(0).GetChild(0).localScale.y;
        v3T.z = 0.1f / line.transform.GetChild(0).GetChild(0).localScale.z;
        //Change Scale/Rotation of the Line-GameObject without affecting Scale of the Text
        line.transform.GetChild(0).localScale = v3T;
        line.transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector3.right, point2 - point1);

        //string letter = ((Char)(64 + lineFact.Id + 1)).ToString();
        //line.GetComponentInChildren<TextMeshPro>().text = letter;
        line.GetComponentInChildren<TextMeshPro>().text = ((Char)(64 + pointFact1.Id + 1)).ToString() + ((Char)(64 + pointFact2.Id + 1)).ToString();
        line.GetComponentInChildren<TextMeshPro>().text += " = " + Math.Round((point1-point2).magnitude, 2) + " m";
        line.GetComponentInChildren<FactObject>().Id = lineFact.Id;
        lineFact.Representation = line;
        return lineFact;

    }

    public Fact SpawnRay(Fact fact)
    {
        RayFact rayFact = ((RayFact)fact);

        PointFact pointFact1 = (Facts[rayFact.Pid1] as PointFact);
        PointFact pointFact2 = (Facts[rayFact.Pid2] as PointFact);

 
        Vector3 point1 = pointFact1.Point;
        Vector3 point2 = pointFact2.Point;

        Vector3 dir = (point2 - point1).normalized;
        point1 -= dir * 100;
        point2 += dir * 100;

         //Change FactRepresentation to Line
         this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Ray", typeof(GameObject));
        GameObject line = GameObject.Instantiate(FactRepresentation);
        //Place the Line in the centre of the two points
        line.transform.position = Vector3.Lerp(point1, point2, 0.5f);
        //Change scale and rotation, so that the two points are connected by the line
        //Get the Line-GameObject as the first Child of the Line-Prefab -> That's the Collider
        var v3T = line.transform.GetChild(0).localScale;
        v3T.x = (point2 - point1).magnitude;
        Debug.Log(v3T.x);
        //For every Coordinate x,y,z we have to devide it by the LocalScale of the Child,
        //because actually the Child should be of this length and not the parent, which is only the Collider
        v3T.x = v3T.x / line.transform.GetChild(0).GetChild(0).localScale.x;
        //y and z of the line/Cube-GameObject here hard coded = ratio of sphere-prefab
        v3T.y = 0.1f / line.transform.GetChild(0).GetChild(0).localScale.y;
        v3T.z = 0.1f / line.transform.GetChild(0).GetChild(0).localScale.z;
        //Change Scale/Rotation of the Line-GameObject without affecting Scale of the Text
        line.transform.GetChild(0).localScale = v3T;
        line.transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector3.right, point2 - point1);

        string letter = ((Char)(64 + rayFact.Id + 1)).ToString();
        line.GetComponentInChildren<TextMeshPro>().text = letter;
     
        line.GetComponentInChildren<FactObject>().Id = rayFact.Id;
        rayFact.Representation = line;
        return rayFact;
    }
    
    //Spawn an angle: point with id = angleFact.Pid2 is the point where the angle gets applied
    public Fact SpawnAngle(Fact fact)
    {
        AngleFact angleFact = (AngleFact)fact;

        Vector3 point1 = (Facts[angleFact.Pid1] as PointFact).Point;
        Vector3 point2 = (Facts[angleFact.Pid2] as PointFact).Point;
        Vector3 point3 = (Facts[angleFact.Pid3] as PointFact).Point;

        Vector3 tempPoint1;
        Vector3 tempPoint3;

        //Length of the Angle relative to the Length of the shortest of the two lines (point2->point1) and (point2->point3)
        float lengthFactor = 0.3f;
        //AngleGO: Triangle-Length: 3/4, Circle-Length: 1/4
        float angleGoFactorTriangleToCircle = 1.33f;// 1.27f;

        //Make 2 TempPoints positioned on length% from Point2 to Point3 and on length% from Point2 to Point1
        //Will be used for z-Coordinate of the Angle
        float length = 0;
        if ((point1 - point2).magnitude >= (point3 - point2).magnitude)
        {
            length = lengthFactor * (point3 - point2).magnitude;
            tempPoint1 = point2 + length * (point1 - point2).normalized;
            tempPoint3 = point2 + length * (point3 - point2).normalized;
        }
        else
        {
            length = lengthFactor * (point1 - point2).magnitude;
            tempPoint1 = point2 + length * (point1 - point2).normalized;
            tempPoint3 = point2 + length * (point3 - point2).normalized;
        }

        //Change FactRepresentation to Angle
        this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Angle", typeof(GameObject));
        GameObject angle = GameObject.Instantiate(FactRepresentation);

        //Place the Angle at position of point2
        angle.transform.position = point2;

        //Change scale and rotation, so that the angle is in between the two lines
        var v3T = angle.transform.localScale;
        //Calculate the Vector from point 2 to a POINT, where (point2->POINT) is orthogonal to (POINT->tempPoint3)
        Vector3 tempProjection = Vector3.Project((tempPoint3 - point2), (Vector3.Lerp((tempPoint1 - point2).normalized, (tempPoint3 - point2).normalized, 0.5f)));

        //Make the Angle as long as length + length of the half-circle
        v3T.x = (tempProjection).magnitude * angleGoFactorTriangleToCircle;

        //For every Coordinate x,y,z we have to devide it by the LocalScale of the Child,
        //because actually the Child should be of this length and not the parent, which is only the Collider
        v3T.x = v3T.x / angle.transform.GetChild(0).GetChild(0).localScale.x;

        //y of the angle-GameObject here hard coded = ratio of sphere-prefab
        v3T.y = 0.05f / angle.transform.GetChild(0).GetChild(0).localScale.y;

        //z should be as long as the distance between tempPoint1 and tempPoint3
        v3T.z = (tempPoint3 - tempPoint1).magnitude / angle.transform.GetChild(0).GetChild(0).localScale.z;

        //Change Scale/Rotation of the Line-GameObject without affecting Scale of the Text
        angle.transform.GetChild(0).localScale = v3T;

        //Rotate so that the rotation points from point2 to the middle of point3 and point1
        angle.transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector3.right, (Vector3.Lerp((tempPoint1 - point2).normalized, (tempPoint3 - point2).normalized, 0.5f)));
        //Now the rotation around that direction must also be adjusted
        //We calculate the Angle not with Vector3.Angle() because it only returns absolute angle-values
        float signedAngle = Mathf.Atan2(Vector3.Dot((Vector3.Lerp((tempPoint1 - point2).normalized, (tempPoint3 - point2).normalized, 0.5f)), Vector3.Cross(angle.transform.GetChild(0).forward.normalized, (tempPoint1 - tempPoint3).normalized)), Vector3.Dot(angle.transform.GetChild(0).forward.normalized, (tempPoint1 - tempPoint3).normalized)) * Mathf.Rad2Deg;
        if (signedAngle < 0)
        {
            angle.transform.GetChild(0).RotateAround(point2, (Vector3.Lerp((tempPoint1 - point2).normalized, (tempPoint3 - point2).normalized, 0.5f)), Vector3.Angle(angle.transform.GetChild(0).forward.normalized, (tempPoint3 - tempPoint1).normalized));
        }
        else
            angle.transform.GetChild(0).RotateAround(point2, (Vector3.Lerp((tempPoint1 - point2).normalized, (tempPoint3 - point2).normalized, 0.5f)), Vector3.Angle(angle.transform.GetChild(0).forward.normalized, (tempPoint1 - tempPoint3).normalized));

        //string letter = ((Char)(64 + angleFact.Id + 1)).ToString();
        //Don't need next line anymore: Cause Text is now not above, but in the centre of the angle
        //angle.GetComponentInChildren<TextMeshPro>().transform.localPosition = new Vector3((0.5f * tempProjection).x, angle.GetComponentInChildren<TextMeshPro>().transform.localPosition.y, (0.5f * tempProjection).z);
        
        TextMeshPro[] texts = angle.GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro t in texts) {
            //Change Text not to the id, but to the angle-value (from both sides) AND change font-size relative to length of the angle (from both sides)
            t.text = Math.Abs(Math.Round(Vector3.Angle((point1 - point2).normalized, (point3 - point2).normalized), 1)) + "°";
            t.fontSize = angle.GetComponentInChildren<TextMeshPro>().fontSize * angle.transform.GetChild(0).transform.GetChild(0).localScale.x;
        }

        angle.GetComponentInChildren<FactObject>().Id = angleFact.Id;
        angleFact.Representation = angle;
        return angleFact;
    }

    public void DeleteObject(Fact fact)
    {
        Debug.Log("delete obj of "+ fact.Id);
        GameObject factRepresentation = fact.Representation;
        GameObject.Destroy(factRepresentation);
    }

    public void animateNonExistingFactTrigger(Fact fact) {
        StartCoroutine(animateNonExistingFact(fact));
    }

    public IEnumerator animateNonExistingFact(Fact fact) {
        Fact returnedFact = getAction(fact)?.Invoke(fact);
        if (returnedFact != null)
        {
            Animator animator = returnedFact.Representation.GetComponentInChildren<Animator>();
            animator.SetTrigger("animateHint");

            yield return new WaitForSeconds(7);

            GameObject.Destroy(returnedFact.Representation);
        }
    }
}
