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

        this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Sphere", typeof(GameObject));
     
        GameObject point = GameObject.Instantiate(FactRepresentation);
        point.transform.position = fact.Point;
        point.transform.up = fact.Normal;
        string letter = ((Char)(64+fact.Id+1)).ToString();
        point.GetComponentInChildren<TextMeshPro>().text = letter;
        point.GetComponent<FactObject>().Id = fact.Id;
        fact.Representation = point;

        //If a new Point was spawned -> We are in MarkPointMode -> Then we want the collider to be disabled
        //Hint: Thats why by now, if we mark a Point in an other mode than MarkPointMode, the 
        //Collider will be set disabled
        if(CommunicationEvents.ActiveToolMode != ToolMode.ExtraMode)
            point.GetComponentInChildren<SphereCollider>().enabled = false;
    

    }

    public void SpawnLine(LineFact lineFact)
    {

        Vector3 point1 = (Facts[lineFact.Pid1] as PointFact).Point;
        Vector3 point2 = (Facts[lineFact.Pid2] as PointFact).Point;
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

        string letter = ((Char)(64 + lineFact.Id + 1)).ToString();
        line.GetComponentInChildren<TextMeshPro>().text = letter;
        line.GetComponentInChildren<FactObject>().Id = lineFact.Id;
        //If a new Line was spawned -> We are in CreateLineMode -> Then we want the collider to be disabled
        if (CommunicationEvents.ActiveToolMode != ToolMode.ExtraMode)
            //Deactivate the Collider of the Line itself
            line.transform.GetComponentInChildren<BoxCollider>().enabled = false;
        lineFact.Representation = line;

    }

    public void DeleteObject(Fact fact)
    {
        Debug.Log("delete obj");
        GameObject factRepresentation = fact.Representation;
        GameObject.Destroy(factRepresentation);
   
    }
}
