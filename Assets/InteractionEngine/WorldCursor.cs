using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldCursor : MonoBehaviour
{


    private RaycastHit Hit;
    private Camera Cam;

    void Start()
    {
        Cam = Camera.main;
 
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


        
    }

    void CheckMouseButtons()
    {
        //send HitEvent
        if (Input.GetMouseButtonDown(0)){
             CommunicationEvents.TriggerEvent.Invoke(Hit);
        }

    }

 

}
