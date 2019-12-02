using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldCursor : MonoBehaviour
{
    private RaycastHit Hit;
    private Camera Cam;
    private ToolMode ActiveToolMode{get; set;}

    void Start()
    {
        Cam = Camera.main;
        //Set MarkPointMode as the default ActiveToolMode
        this.ActiveToolMode = ToolMode.MarkPointMode;
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

            CheckMouseButtons();

        }
        else
        {
            transform.position = Cam.transform.position + Cam.transform.forward * 10;
            transform.up = -Cam.transform.forward;
        }

        //Check if the ToolMode was switched
        CheckToolModeSelection();
        
    }

    void CheckMouseButtons()
    {
        //send HitEvent
        if (Input.GetMouseButtonDown(0)){
             CommunicationEvents.TriggerEvent.Invoke(Hit);
        }

    }

    void CheckToolModeSelection() {
        if (Input.GetButtonDown("ToolMode")) {
            //Change the ActiveToolMode dependent on which Mode was selected
            if ((int)this.ActiveToolMode == Enum.GetNames(typeof(ToolMode)).Length - 1)
            {
                this.ActiveToolMode = 0;
            }
            else {
                this.ActiveToolMode++;
            }

            //Invoke the Handler for the Facts
            CommunicationEvents.ToolModeChangedEvent.Invoke(this.ActiveToolMode);
        }
    }

 

}
