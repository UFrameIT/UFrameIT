using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FactSpawner : MonoBehaviour
{
    public GameObject FactRepresentation;
    public string[] Facts = new String[100];
    public GameObject[] GameObjectFacts = new GameObject[100];

    void Start()
    {
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        CommunicationEvents.AddEvent.AddListener(SpawnFact);
        CommunicationEvents.RemoveEvent.AddListener(DeletePoint);
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

    public void SpawnFact(RaycastHit hit, int id) {
        SpawnPoint(hit, id);
    }


    public void SpawnPoint(RaycastHit hit, int id)
    {
        Debug.Log(id);
        GameObject point = GameObject.Instantiate(FactRepresentation);
        point.transform.position = hit.point;
        point.transform.up = hit.normal;
        string letter = ((Char)(64+id+1)).ToString();
        point.GetComponentInChildren<TextMeshPro>().text = letter;
        //If a new Point was spawned -> We are in MarkPointMode -> Then we want the collider to be disabled
        point.GetComponentInChildren<SphereCollider>().enabled = false;
        Facts[id] = letter;
        GameObjectFacts[id] = point;

    }

    public void DeletePoint(RaycastHit hit, int id)
    {
        GameObject point = hit.transform.gameObject;
        GameObject.Destroy(point);
        Facts[id] = "";
    }

    public void OnHit(RaycastHit hit)
    {

        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
        {
            //hit existing point, so delete it
            char letter = hit.transform.gameObject.GetComponentInChildren<TextMeshPro>().text.ToCharArray()[0];
            int id = letter - 65;
            CommunicationEvents.RemoveEvent.Invoke(hit, id);
        }
        else
        {

            CommunicationEvents.AddEvent.Invoke(hit, GetFirstEmptyID());
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
                    if (GameObjectFact.tag == "Line" || GameObjectFact.tag == "Angle")
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (GameObjectFact.tag == "Point") {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.CreateAngleMode:
            //If CreateAngleMode is activated we want to have the ability to select Lines for the Angle
            //but we don't want to have the ability to select Points or Angles
                foreach (GameObject GameObjectFact in this.GameObjectFacts)
                {
                    if (GameObjectFact.tag == "Point" || GameObjectFact.tag == "Angle")
                    {
                        GameObjectFact.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (GameObjectFact.tag == "Line")
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
        }
    }

      

}
