using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class WorldCursor : MonoBehaviour
{
    private RaycastHit Hit;
    private Camera Cam;
    private ToolMode ActiveToolMode{get; set;}

    private string selectableTag = "Selectable";
    private Transform lastFactSelection;


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

            //SELECTION-HIGHLIGHTING-PART
            //Check if a Fact was Hit
            Transform selection = Hit.transform;

            //Set the last Fact unselected
            if (this.lastFactSelection != null)
            {
                //Invoke the EndHighlightEvent that will be handled in FactSpawner
                CommunicationEvents.EndHighlightEvent.Invoke(this.lastFactSelection);
                this.lastFactSelection = null;
            }

            //Set the Fact that was Hit as selected
            if (selection.CompareTag(selectableTag))
            {
                //Invoke the HighlightEvent that will be handled in FactSpawner
                this.lastFactSelection = selection;
                CommunicationEvents.HighlightEvent.Invoke(selection);
            }
            //SELECTION-HIGHLIGHTING-PART-END

            CheckMouseButtons();

        }
        else
        {
            transform.position = Cam.ScreenToWorldPoint(Input.mousePosition);
            transform.up = -Cam.transform.forward;
        }

        //Check if the ToolMode was switched
        CheckToolModeSelection();
        
    }

    void CheckMouseButtons()
    {
       
        //send HitEvent
        if (Input.GetMouseButtonDown(0)){
            if (EventSystem.current.IsPointerOverGameObject()) return;
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
