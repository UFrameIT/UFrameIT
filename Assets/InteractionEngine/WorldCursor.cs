using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WorldCursor : MonoBehaviour
{
    public RaycastHit Hit;
    private Camera Cam;
    private ToolMode ActiveToolMode{get; set;}



    void Start()
    {

        Cam = Camera.main;
        //Set MarkPointMode as the default ActiveToolMode
        this.ActiveToolMode = ToolMode.ExtraMode;//ToolMode.MarkPointMode;
        CommunicationEvents.ToolModeChangedEvent.Invoke(this.ActiveToolMode);
        //TODO: we probably can configure these things to automatically trigger when the variable is changed...
        CommunicationEvents.ActiveToolMode = this.ActiveToolMode;
        //redundant for now, but we probably want to have the activetool mode available globally
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

    //Deactivate LineRenderer so that no Line gets drawn when Cursor changes


    //Check if left Mouse-Button was pressed and handle it
    void CheckMouseButtons(Ray ray)
    {
        if (Input.GetMouseButtonDown(0))
        {
          
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
            CommunicationEvents.ActiveToolMode = this.ActiveToolMode;
            //Invoke the Handler for the Facts
            CommunicationEvents.ToolModeChangedEvent.Invoke(this.ActiveToolMode);
        }
    }

 

}
