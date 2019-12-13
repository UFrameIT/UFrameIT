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

        //Default FactRepresenation = Sphere-Prefab for Points
        this.FactRepresentation = (GameObject) Resources.Load("Prefabs/Sphere", typeof(GameObject));

    }

    public void FactAction(Fact fact)
    {
     
        switch  (fact)
        {
            case PointFact pointFact:
                SpawnPoint(pointFact);
                break;
            case LineFact lineFact:
                SpawnLine(lineFact);
                break;

        }
    }
  

    public void SpawnPoint(PointFact fact)
    {
       

        PointFact pointFact = fact;
        this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Sphere", typeof(GameObject));
     
        GameObject point = GameObject.Instantiate(FactRepresentation);
        point.transform.position = pointFact.Point;
        point.transform.up = pointFact.Normal;
        string letter = ((Char)(64+fact.Id+1)).ToString();
        point.GetComponentInChildren<TextMeshPro>().text = letter;
        point.GetComponent<FactObject>().Id = fact.Id;
        pointFact.Representation = point;

        //If a new Point was spawned -> We are in MarkPointMode -> Then we want the collider to be disabled
        //Hint: Thats why by now, if we mark a Point in an other mode than MarkPointMode, the 
        //Collider will be set disabled
        if(CommunicationEvents.ActiveToolMode != ToolMode.ExtraMode)
            point.GetComponentInChildren<SphereCollider>().enabled = false;
    

    }

    public void DeleteObject(Fact fact)
    {
        Debug.Log("delete obj");
        GameObject point = fact.Representation;
        GameObject.Destroy(point);
   
    }

    public void SpawnLine(LineFact lineFact) {


        Vector3 point1 = (Facts[lineFact.Pid1] as PointFact).Point;
        Vector3 point2 = (Facts[lineFact.Pid2] as PointFact).Point;
        //Change FactRepresentation to Line
        this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Line2", typeof(GameObject));
        GameObject line = GameObject.Instantiate(FactRepresentation);
        //Place the Line in the centre of the two points
        //and change scale and rotation, so that the two points are connected by the line
        line.transform.position = Vector3.Lerp(point1, point2, 0.5f);
        var v3T = line.transform.localScale;
        v3T.y = (point2 - point1).magnitude;
        //x and z of the line/Cube-GameObject here hard coded = ratio of sphere-prefab
        v3T.x = 0.1f;
        v3T.z = 0.1f;
        line.transform.localScale = v3T;
        line.transform.rotation = Quaternion.FromToRotation(Vector3.up, point2 - point1);

        string letter = ((Char)(64 + lineFact.Id + 1)).ToString();
        line.GetComponentInChildren<TextMeshPro>().text = letter;
        line.GetComponent<FactObject>().Id = lineFact.Id;
        //If a new Line was spawned -> We are in CreateLineMode -> Then we want the collider to be disabled
        if (CommunicationEvents.ActiveToolMode != ToolMode.ExtraMode)
            line.GetComponentInChildren<BoxCollider>().enabled = false;
        lineFact.Representation = line;
     
    }



 

    

}
