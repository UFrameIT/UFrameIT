using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CommunicationEvents;

public class FactSpawner : MonoBehaviour
{
    private GameObject FactRepresentation;


    //Variables for highlighting Facts where the cursor moves over
    public Material defaultMaterial;
    public Material highlightMaterial;

    public GameObject SmartMenu;

    void Start()
    {
        CommunicationEvents.HighlightEvent.AddListener(OnMouseOverFact);
        CommunicationEvents.EndHighlightEvent.AddListener(OnMouseOverFactEnd);
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
 
        CommunicationEvents.AddPointEvent.AddListener(SpawnPoint);
        CommunicationEvents.AddLineEvent.AddListener(SpawnLine);
        CommunicationEvents.RemoveEvent.AddListener(DeletePoint);

        //Default FactRepresenation = Sphere-Prefab for Points
        this.FactRepresentation = (GameObject) Resources.Load("Prefabs/Sphere", typeof(GameObject));

    }

  

    public void SpawnPoint(RaycastHit hit, int id)
    {
        this.FactRepresentation = (GameObject)Resources.Load("Prefabs/Sphere", typeof(GameObject));
        Debug.Log(id);
        GameObject point = GameObject.Instantiate(FactRepresentation);
        point.transform.position = hit.point;
        point.transform.up = hit.normal;
        string letter = ((Char)(64+id+1)).ToString();
        point.GetComponentInChildren<TextMeshPro>().text = letter;
        point.GetComponent<FactObject>().Id = id;
        //If a new Point was spawned -> We are in MarkPointMode -> Then we want the collider to be disabled
        //Hint: Thats why by now, if we mark a Point in an other mode than MarkPointMode, the 
        //Collider will be set disabled
        if(CommunicationEvents.ActiveToolMode != ToolMode.ExtraMode)
            point.GetComponentInChildren<SphereCollider>().enabled = false;
    

    }

    public void DeletePoint(int id)
    {
        GameObject point = Facts[id].Representation;
        GameObject.Destroy(point);
   
    }

    public void SpawnLine(int pid1, int pid2, int id) {

        Vector3 point1 = (Facts[pid1] as PointFact).Point;
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

        string letter = ((Char)(64 + id + 1)).ToString();
        line.GetComponentInChildren<TextMeshPro>().text = letter;
        line.GetComponent<FactObject>().Id = id;
        //If a new Line was spawned -> We are in CreateLineMode -> Then we want the collider to be disabled
        if (CommunicationEvents.ActiveToolMode != ToolMode.ExtraMode)
            line.GetComponentInChildren<BoxCollider>().enabled = false;
        Facts[id] = letter;
        GameObjectFacts[id] = line;
    }



 

    

}
