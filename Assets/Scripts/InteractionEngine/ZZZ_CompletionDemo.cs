using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static CommunicationEvents;

public class ZZZ_CompletionDemo : MonoBehaviour
{
    private GameObject FactRepresentation;
    private RaycastHit point1;
    private RaycastHit point2;
    private RaycastHit point3;
    private AngleFact angle1;

    public FactManager FactManager;

    // Start is called before the first frame update
    void Start()
    {
        //Default FactRepresenation = Sphere-Prefab for Points
        this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Sphere", typeof(GameObject));
        if (FactManager == null) FactManager = GameObject.FindObjectOfType<FactManager>();

        point1 = new RaycastHit();
        point1.normal = new Vector3(0.0f, 1.0f, 0.0f);
        point1.point = new Vector3(4.1f, 0.0f, 7.6f);

        point2 = new RaycastHit();
        point2.normal = new Vector3(0.0f, 1.0f, 0.0f);
        point2.point = new Vector3(2.6f, 0.0f, 2.2f);

        point3 = new RaycastHit();
        point3.normal = new Vector3(0.0f, 1.0f, 0.0f);
        point3.point = new Vector3(2.6f, 6.3f, 2.2f);

        angle1 = new AngleFact();
        angle1.Id = 3;
        angle1.Pid1 = 0;
        angle1.Pid2 = 1;
        angle1.Pid3 = 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CompletionDemo()
    {
        FactManager.AddPointFact(point1, 0);
        FactManager.AddPointFact(point2, 1);
        FactManager.AddPointFact(point3, 2);

        StartCoroutine(SpawnAngleCoroutine());

    }

    IEnumerator SpawnAngleCoroutine()
    {
        yield return new WaitUntil(() => Facts.Count >= 3);

        StartCoroutine(SpawnAngleAndAnimate(angle1));
    }

    //Spawn an angle: point with id = angleFact.Pid2 is the point where the angle gets applied
    IEnumerator SpawnAngleAndAnimate(AngleFact angleFact)
    {

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
        foreach (TextMeshPro t in texts)
        {
            //Change Text not to the id, but to the angle-value (from both sides) AND change font-size relative to length of the angle (from both sides)
            t.text = Math.Abs(Math.Round(Vector3.Angle((point1 - point2).normalized, (point3 - point2).normalized), 1)) + "°";
            t.fontSize = angle.GetComponentInChildren<TextMeshPro>().fontSize * angle.transform.GetChild(0).transform.GetChild(0).localScale.x;
        }

        angle.GetComponentInChildren<FactObject>().Id = angleFact.Id;
        angleFact.Representation = angle;
        //Test
        Animator temp = angle.GetComponentInChildren<Animator>();
        temp.SetTrigger("animateHint");

        CommunicationEvents.parameterDisplayHint.Invoke("∠ABC");

        yield return new WaitForSeconds(7);

        GameObject.Destroy(angle);
    }
}
