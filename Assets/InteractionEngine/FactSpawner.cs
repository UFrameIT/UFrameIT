using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FactSpawner : MonoBehaviour
{
    public GameObject PointRepresentation;
    public string[] Facts = new String[100];




    void Start()
    {
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
        CommunicationEvents.AddEvent.AddListener(SpawnPoint);
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


    public void SpawnPoint(RaycastHit hit, int id)
    {
        Debug.Log(id);
        GameObject point = GameObject.Instantiate(PointRepresentation);
        point.transform.position = hit.point;
        point.transform.up = hit.normal;
        string letter = ((Char)(64+id+1)).ToString();
        point.GetComponentInChildren<TextMeshPro>().text = letter;
        Facts[id] = letter;
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
            CommunicationEvents.RemoveEvent.Invoke(hit,id);
        }
        else
        {

            CommunicationEvents.AddEvent.Invoke(hit, GetFirstEmptyID());
        }
    }

      

}
