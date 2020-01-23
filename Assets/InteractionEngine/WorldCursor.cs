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
       // ActiveToolMode = ToolMode.ExtraMode;//ToolMode.MarkPointMode;
        CommunicationEvents.ToolModeChangedEvent.Invoke(ActiveToolMode);

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

       
        int layerMask = 1 << LayerMask.NameToLayer("Player"); //only hit player
        layerMask = ~layerMask; //ignore Player

   

        if(Physics.Raycast(ray, out Hit, 30f, layerMask)){

            Debug.Log(Hit.transform.tag);
            if (Hit.collider.transform.CompareTag("SnapZone"))
            {
                Hit.point = Hit.collider.transform.position;
                Hit.normal = Vector3.up;
                CheckMouseButtons(true);
                transform.position = Hit.point;
                transform.up = Hit.normal;

            }
            else
            {
                transform.position = Hit.point;
                transform.up = Hit.normal;
                transform.position += .01f * Hit.normal;
                CheckMouseButtons();
            }




        }
        else
        {
            var dist = 10f;
            if (Hit.transform!=null)
            dist = (Camera.main.transform.position - Hit.transform.position).magnitude;
            transform.position = Cam.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,1) *dist);
            transform.up = -Cam.transform.forward;
        }


        
    }

    //Check if left Mouse-Button was pressed and handle it
    void CheckMouseButtons(bool OnSnap=false)
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; //this prevents rays from shooting through ui

            if (!OnSnap)
            {
                CommunicationEvents.TriggerEvent.Invoke(Hit);
            }
            else {
                Hit.collider.enabled = false;
                CommunicationEvents.TriggerEvent.Invoke(Hit);
            //    CommunicationEvents.SnapEvent.Invoke(Hit);
            }
                

          

        }
    }

   



 

}
