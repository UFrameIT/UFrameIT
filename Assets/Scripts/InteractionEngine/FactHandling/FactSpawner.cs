using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static CommunicationEvents;
using static GlobalBehaviour;

public class FactSpawner : MonoBehaviour
{
    public GameObject
        Sphere,
        Line,
        Ray,
        Angle,
        Ring;

    private GameObject FactRepresentation;
    //private Camera camera;

    void Start()
    {
       
        AddFactEvent.AddListener(FactAction);
        RemoveFactEvent.AddListener(DeleteObject);

        AnimateNonExistingFactEvent.AddListener(animateNonExistingFactTrigger);

        //Default FactRepresenation = Sphere-Prefab for Points
        this.FactRepresentation = Sphere;

        //camera = Camera.main;

    }

    public void FactAction(Fact fact) {
        getAction(fact)?.Invoke(fact);
    }

    public Func<Fact, Fact> getAction(Fact fact)
    {
        return fact switch
        {
            PointFact pointFact => SpawnPoint,
            LineFact lineFact => SpawnLine,
            AngleFact angleFact => SpawnAngle,
            RayFact rayFact => SpawnRay,
            CircleFact circleFact => SpawnRing,
            _ => null,
        };
    }
  

    public Fact SpawnPoint(Fact pointFact)
    {
        PointFact fact = ((PointFact)pointFact);
        this.FactRepresentation = Sphere;
     
        GameObject point = GameObject.Instantiate(FactRepresentation);
        point.transform.position = fact.Point;
        point.transform.up = fact.Normal;
        point.GetComponentInChildren<TextMeshPro>().text = fact.Label;
        point.GetComponent<FactObject>().URI = fact.Id;
        fact.Representation = point;
        return fact;
    }

    public Fact SpawnLine(Fact fact)
    {
        LineFact lineFact = ((LineFact)fact);

        PointFact pointFact1 = (StageStatic.stage.factState[lineFact.Pid1] as PointFact);
        PointFact pointFact2 = (StageStatic.stage.factState[lineFact.Pid2] as PointFact);
        Vector3 point1 = pointFact1.Point;
        Vector3 point2 = pointFact2.Point;
        //Change FactRepresentation to Line
        this.FactRepresentation = Line;
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
        line.GetComponentInChildren<TextMeshPro>().text = pointFact1.Label + pointFact2.Label;
        line.GetComponentInChildren<TextMeshPro>().text += " = " + Math.Round((point1-point2).magnitude, 2) + " m";
        line.GetComponentInChildren<FactObject>().URI = lineFact.Id;
        lineFact.Representation = line;
        return lineFact;

    }

    public Fact SpawnRay(Fact fact)
    {
        RayFact rayFact = ((RayFact)fact);

        PointFact pointFact1 = (StageStatic.stage.factState[rayFact.Pid1] as PointFact);
        PointFact pointFact2 = (StageStatic.stage.factState[rayFact.Pid2] as PointFact);

 
        Vector3 point1 = pointFact1.Point;
        Vector3 point2 = pointFact2.Point;

        Vector3 dir = (point2 - point1).normalized;
        point1 -= dir * 100;
        point2 += dir * 100;

        //Change FactRepresentation to Line
        this.FactRepresentation = Ray;
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

        line.GetComponentInChildren<TextMeshPro>().text = rayFact.Label;
        line.GetComponentInChildren<FactObject>().URI = rayFact.Id;

        rayFact.Representation = line;
        return rayFact;
    }
    
    //Spawn an angle: point with id = angleFact.Pid2 is the point where the angle gets applied
    public Fact SpawnAngle(Fact fact)
    {
        AngleFact angleFact = (AngleFact)fact;

        Vector3 point1 = (StageStatic.stage.factState[angleFact.Pid1] as PointFact).Point;
        Vector3 point2 = (StageStatic.stage.factState[angleFact.Pid2] as PointFact).Point;
        Vector3 point3 = (StageStatic.stage.factState[angleFact.Pid3] as PointFact).Point;

        //Length of the Angle relative to the Length of the shortest of the two lines (point2->point1) and (point2->point3)
        float lengthFactor = 0.3f;
        
        float length;
        if ((point1 - point2).magnitude >= (point3 - point2).magnitude)
            length = lengthFactor * (point3 - point2).magnitude;
        else
            length = lengthFactor * (point1 - point2).magnitude;

        //Change FactRepresentation to Angle
        this.FactRepresentation = Angle;
        GameObject angle = GameObject.Instantiate(FactRepresentation);

        //Calculate Angle:
        Vector3 from = (point3 - point2).normalized;
        Vector3 to = (point1 - point2).normalized;
        float angleValue = Vector3.Angle(from, to); //We always get an angle between 0 and 180° here

        //Change scale and rotation, so that the angle is in between the two lines
        var v3T = angle.transform.localScale;
        v3T = new Vector3(length, v3T.y, length);

        Vector3 up = Vector3.Cross(to, from);
        //Place the Angle at position of point2
        angle.transform.SetPositionAndRotation(point2, Quaternion.LookRotation(Vector3.Cross((from+to).normalized,up), up));

        //Set text of angle
        TextMeshPro[] texts = angle.GetComponentsInChildren<TextMeshPro>();
        foreach (TextMeshPro t in texts) {
            //Change Text not to the id, but to the angle-value (from both sides) AND change font-size relative to length of the angle (from both sides)
            t.text = Math.Round((double) angleValue, 2) + "°";
            t.fontSize = angle.GetComponentInChildren<TextMeshPro>().fontSize * angle.transform.GetChild(0).transform.GetChild(0).localScale.x;
        }

        //Generate angle mesh
        CircleSegmentGenerator[] segments = angle.GetComponentsInChildren<CircleSegmentGenerator>();
        foreach (CircleSegmentGenerator c in segments)
            c.setAngle(angleValue);

        angle.GetComponentInChildren<FactObject>().URI = angleFact.Id;
        angleFact.Representation = angle;
        return angleFact;
    }

    public Fact SpawnRing(Fact fact)
    {
        CircleFact circleFact = (CircleFact)fact;

        PointFact middlePointFact = StageStatic.stage.factState[circleFact.Pid1] as PointFact;
        PointFact basePointFact = StageStatic.stage.factState[circleFact.Pid2] as PointFact;

        Vector3 middlePoint = middlePointFact.Point;
        Vector3 normal = circleFact.normal;
        float radius = circleFact.radius;

        //Change FactRepresentation to Ring
        this.FactRepresentation = Ring;
        GameObject ring = Instantiate(FactRepresentation);

        var torus = ring.GetComponent<TorusGenerator>();
        var tmpText = ring.GetComponentInChildren<TextMeshPro>();
        var FactObj = ring.GetComponentInChildren<FactObject>();

        //Move Ring to middlePoint
        ring.transform.position = middlePoint;

        //Rotate Ring according to normal
        ring.transform.rotation = Quaternion.LookRotation(Vector3.forward, normal); // TODO TSc works?

        //Set radii
        torus.torusRadius = radius;

        //string letter = ((Char)(64 + lineFact.Id + 1)).ToString();
        //line.GetComponentInChildren<TextMeshPro>().text = letter;
        string text = 
            $"\u25EF {middlePointFact.Label}{basePointFact.Label}" +
            $"r = {radius}, C = {2 * radius * Math.PI}";
        tmpText.text = text;
        //move TMP Text so it is on the edge of the circle
        tmpText.rectTransform.position = tmpText.rectTransform.position - new Vector3(0, 0, -radius);

        FactObj.URI = circleFact.Id;
        circleFact.Representation = ring;

        return circleFact;
    }


    public void DeleteObject(Fact fact)
    {
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
            MeshRendererHintAnimation animator = returnedFact.Representation.GetComponentInChildren<MeshRendererHintAnimation>();
            animator.AnimationTrigger();

            yield return new WaitForSeconds(GlobalBehaviour.hintAnimationDuration);

            GameObject.Destroy(returnedFact.Representation);
        }
    }
}
