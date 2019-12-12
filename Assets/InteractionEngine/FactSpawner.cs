using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FactSpawner : MonoBehaviour
{
    private GameObject FactRepresentation;
    public string[] Facts = new String[100];
    public GameObject[] GameObjectFacts = new GameObject[100];

    //Variables for highlighting Facts where the cursor moves over
    public Material defaultMaterial;
    public Material highlightMaterial;

    public GameObject SmartMenu;

    void Start()
    {
        CommunicationEvents.HighlightEvent.AddListener(OnMouseOverFact);
        CommunicationEvents.EndHighlightEvent.AddListener(OnMouseOverFactEnd);
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        CommunicationEvents.AddPointEvent.AddListener(SpawnPoint);
        CommunicationEvents.AddLineEvent.AddListener(SpawnLine);
        CommunicationEvents.RemoveEvent.AddListener(DeletePoint);

        //Default FactRepresenation = Sphere-Prefab for Points
        this.FactRepresentation = (GameObject) Resources.Load("Prefabs/Sphere", typeof(GameObject));

    }

    public int GetFirstEmptyID()
    {
       
        for(int i = 0; i < Facts.Length; ++i)
        {
            if(Facts[i]== "")
                return i;
        }
        return Facts.Length - 1;
   
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
        Facts[id] = letter;
        GameObjectFacts[id] = point;

    }

    public void DeletePoint(int id)
    {
        GameObject point = GameObjectFacts[id];
        GameObject.Destroy(point);
        Facts[id] = "";
    }

    public void SpawnLine(Vector3 point1, Vector3 point2) {
        int id = GetFirstEmptyID();
        Debug.Log(id);
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

    public void OnMouseOverFactEnd(Transform selection)
    {
        Renderer selectionRenderer;
        
        if (selection != null)
        {
            selectionRenderer = selection.GetComponent<Renderer>();
            if (selectionRenderer != null)
            {
                //Set the Material of the fact back to default
                selectionRenderer.material = defaultMaterial;
            }
        }
    }

    public void OnMouseOverFact(Transform selection)
    {
        Renderer selectionRenderer;
        
        selectionRenderer = selection.GetComponent<Renderer>();
        if (selectionRenderer != null) {
            //Set the Material of the Fact, where the mouse is over, to a special one
            selectionRenderer.material = highlightMaterial;
        }
    }

    public void OnHit(RaycastHit hit)
    {
        Debug.Log(CommunicationEvents.ActiveToolMode);
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            //hit existing point, so delete it
            if(CommunicationEvents.ActiveToolMode == ToolMode.ExtraMode)
            {
                var menu =GameObject.Instantiate(SmartMenu);
                menu.GetComponent<Canvas>().worldCamera = Camera.main;
                menu.transform.SetParent(hit.transform);
                menu.transform.localPosition = Vector3.up- Camera.main.transform.forward;
            }
            else
            {
                char letter = hit.transform.gameObject.GetComponentInChildren<TextMeshPro>().text.ToCharArray()[0];
                int id = letter - 65;
                CommunicationEvents.RemoveEvent.Invoke(id);
            }

        }
        else
        {

            CommunicationEvents.AddPointEvent.Invoke(hit, GetFirstEmptyID());
        }
    }

    public void OnToolModeChanged(ToolMode ActiveToolMode) {
        switch (ActiveToolMode) {
            case ToolMode.MarkPointMode:
            //If MarkPointMode is activated we want to have the ability to mark the point
            //everywhere, independent of already existing facts
                foreach (GameObject GameObjectFact in this.GameObjectFacts) {
                    GameObjectFact.GetComponentInChildren<Collider>().enabled = false;
                }
                break;
            case ToolMode.CreateLineMode:
            //If CreateLineMode is activated we want to have the ability to select points for the Line
            //but we don't want to have the ability to select Lines or Angles
                foreach (GameObject GameObjectFact in this.GameObjectFacts)
                {
                    if (GameObjectFact.layer == LayerMask.NameToLayer("Line") || GameObjectFact.layer == LayerMask.NameToLayer("Angle"))
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (GameObjectFact.layer == LayerMask.NameToLayer("Point")) {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.CreateAngleMode:
            //If CreateAngleMode is activated we want to have the ability to select Lines for the Angle
            //but we don't want to have the ability to select Points or Angles
                foreach (GameObject GameObjectFact in this.GameObjectFacts)
                {
                    if (GameObjectFact.layer == LayerMask.NameToLayer("Point") || GameObjectFact.layer == LayerMask.NameToLayer("Angle"))
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (GameObjectFact.layer == LayerMask.NameToLayer("Line"))
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.DeleteMode:
            //If DeleteMode is activated we want to have the ability to delete every Fact
            //independent of the concrete type of fact
                foreach (GameObject GameObjectFact in this.GameObjectFacts)
                {
                    GameObjectFact.GetComponentInChildren<Collider>().enabled = true;
                }
                break;
                case ToolMode.ExtraMode:
                foreach (GameObject GameObjectFact in this.GameObjectFacts)
                {
                   
                }
                break;

            

        }
    }

      

}
