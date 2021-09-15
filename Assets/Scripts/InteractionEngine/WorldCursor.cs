using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static GadgetManager;

public class WorldCursor : MonoBehaviour
{
    public RaycastHit Hit;
    public string deactivateSnapKey;
    private Camera Cam;
    private int layerMask;
    public LayerMask snapLayerMask;
    public float MaxRange = 10f;
    public bool useCamCurser = false;

    private void Awake()
    {
        this.layerMask = LayerMask.GetMask("Player", "TalkingZone");
        //Ignore player and TalkingZone
        this.layerMask = ~this.layerMask;
    }

    void Start()
    {
        Cam = Camera.main;
        //Set MarkPointMode as the default ActiveToolMode
       // ActiveToolMode = ToolMode.ExtraMode;//ToolMode.MarkPointMode;
       // CommunicationEvents.ToolModeChangedEvent.Invoke(activeGadget.id);
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
    }

    public void setLayerMask(int layerMask) {
        this.layerMask = layerMask;
    }

    void Update()
    {
        Ray ray = useCamCurser ? new Ray(Cam.transform.position, Cam.transform.forward) : Cam.ScreenPointToRay(Input.mousePosition);

        this.Hit = new RaycastHit();
        transform.up = Cam.transform.forward;
        transform.position = ray.GetPoint(GlobalBehaviour.GadgetPhysicalDistance);

        int rayCastMask;
        if (Input.GetButton(this.deactivateSnapKey))
            rayCastMask = this.layerMask & ~this.snapLayerMask.value;
        else
            rayCastMask = this.layerMask;

        if (Physics.Raycast(ray, out Hit, MaxRange, rayCastMask)
            || (MaxRange <= GlobalBehaviour.GadgetPhysicalDistance 
            && Physics.Raycast(transform.position, Vector3.down, out Hit, GlobalBehaviour.GadgetPhysicalDistance, rayCastMask)))
        {
            if ((Hit.collider.transform.CompareTag("SnapZone") || Hit.collider.transform.CompareTag("Selectable")) 
                && !Input.GetButton(this.deactivateSnapKey))
            {
                if(Hit.collider.gameObject.layer == LayerMask.NameToLayer("Ray")
                    || Hit.collider.gameObject.layer == LayerMask.NameToLayer("Line"))
                {
                    var id = Hit.collider.gameObject.GetComponent<FactObject>().URI;
                    AbstractLineFact lineFact = StageStatic.stage.factState[id] as AbstractLineFact;
                    PointFact p1 =  StageStatic.stage.factState[lineFact.Pid1] as PointFact;

                    Hit.point = Math3d.ProjectPointOnLine(p1.Point, lineFact.Dir, Hit.point);
                }
                else
                {
                    Hit.point = Hit.collider.transform.position;
                    Hit.normal = Vector3.up;
                }

                transform.position = Hit.point;
                transform.up = Hit.normal;

            }
            else
            {
                transform.position = Hit.point;
                transform.up = Hit.normal;
                transform.position += .01f * Hit.normal;
            }

            CheckMouseButtons();

        }
    }

    //Check if left Mouse-Button was pressed and handle it
    void CheckMouseButtons()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() //this prevents rays from shooting through ui
                || Hit.transform.gameObject.layer == LayerMask.NameToLayer("Water")) // not allowed to meassure on water
                return;

            CommunicationEvents.TriggerEvent.Invoke(Hit);
        }
    }

   



 

}
