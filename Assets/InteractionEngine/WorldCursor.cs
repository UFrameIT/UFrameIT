using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static CommunicationEvents;

public class WorldCursor : MonoBehaviour
{
    public RaycastHit Hit;
    private Camera Cam;

    void Start()
    {

        Cam = Camera.main;
        //Set MarkPointMode as the default ActiveToolMode
        ActiveToolMode = ToolMode.ExtraMode;//ToolMode.MarkPointMode;
        CommunicationEvents.ToolModeChangedEvent.Invoke(ActiveToolMode);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

       
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //only hit player
        layerMask = ~layerMask; //ignore Player

   

        if(Physics.Raycast(ray, out Hit, 30f, layerMask)){
            transform.position = Hit.point;
            transform.up = Hit.normal;
            transform.position += .01f * Hit.normal;
            CheckMouseButtons(ray);


        }
        else
        {
            transform.position = Cam.ScreenToWorldPoint(Input.mousePosition);
            transform.up = -Cam.transform.forward;
        }

        //Check if the ToolMode was switched
        CheckToolModeSelection();
        
    }

    //Check if left Mouse-Button was pressed and handle it
    void CheckMouseButtons(Ray ray)
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; //this prevents rays from shooting through ui
       
            CommunicationEvents.TriggerEvent.Invoke(Hit);
              
        }
    }

    //Checks if the ToolMode was switched by User, and handle it
    void CheckToolModeSelection() {
        if (Input.GetButtonDown("ToolMode")) {
            //Change the ActiveToolMode dependent on which Mode was selected
            if ((int)ActiveToolMode == Enum.GetNames(typeof(ToolMode)).Length - 1)
            {
                ActiveToolMode = 0;
            }
            else {
                ActiveToolMode++;
            }
            //Invoke the Handler for the Facts
            CommunicationEvents.ToolModeChangedEvent.Invoke(ActiveToolMode);
        }
    }

 

}
