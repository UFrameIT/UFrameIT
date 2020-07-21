using System;
using System.Globalization;
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
    private int layerMask;

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
        CommunicationEvents.ToolModeChangedEvent.Invoke(ActiveToolMode);
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
    }

    public void setLayerMask(int layerMask) {
        this.layerMask = layerMask;
    }

  
    /// <summary>
    /// Gets the coordinates of the intersection point of two lines.
    /// </summary>
    /// <param name="A1">A point on the first line.</param>
    /// <param name="A2">Another point on the first line.</param>
    /// <param name="B1">A point on the second line.</param>
    /// <param name="B2">Another point on the second line.</param>
    /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
    /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
    public Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
        {
            float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

            if (tmp == 0)
            {
                // No solution!
                found = false;
                return Vector2.zero;
            }

            float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

            found = true;

            return new Vector2(
                B1.x + (B2.x - B1.x) * mu,
                B1.y + (B2.y - B1.y) * mu
            );
        }


    void Update()
    {
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit tempHit;

        if(Physics.Raycast(ray, out tempHit, 30f, this.layerMask)){

            this.Hit = tempHit;
            // Debug.Log(Hit.transform.tag);
            if (Hit.collider.transform.CompareTag("SnapZone"))
            {
                if(Hit.collider.gameObject.layer == LayerMask.NameToLayer("Ray")){

                    int id = Hit.collider.gameObject.GetComponent<FactObject>().Id;
                    RayFact lineFact = CommunicationEvents.Facts.Find((x => x.Id == id)) as RayFact;
                    PointFact p1 =  CommunicationEvents.Facts.Find((x => x.Id == lineFact.Pid1)) as PointFact;
                    PointFact p2 = CommunicationEvents.Facts.Find((x => x.Id == lineFact.Pid2)) as PointFact;

                    Vector3 lineDir = p2.Point - p1.Point;
                    Plane plane = new Plane(lineDir, Hit.point);

                    Ray intersectionRay = new Ray(p1.Point, lineDir );

                    if(plane.Raycast(intersectionRay, out float enter)){

                        Hit.point = p1.Point + lineDir.normalized * enter;
                    }
                    else Debug.LogError("something wrong with linesnapzone");
                    CheckMouseButtons(true,true);



                }
                else
                {
                    Hit.point = Hit.collider.transform.position;
                    Hit.normal = Vector3.up;
                    CheckMouseButtons(true);
                }

            
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
            this.Hit = new RaycastHit();
            var dist = 10f;
            if (tempHit.transform!=null)
            dist = (Camera.main.transform.position - tempHit.transform.position).magnitude;
            transform.position = Cam.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,1) *dist);
            transform.up = -Cam.transform.forward;
        }


        
    }

    //Check if left Mouse-Button was pressed and handle it
    void CheckMouseButtons(bool OnSnap=false, bool onLine = false)
    {
       
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; //this prevents rays from shooting through ui

            if (!OnSnap)
            {
                CommunicationEvents.TriggerEvent.Invoke(Hit);
            }
            else if(CommunicationEvents.ActiveToolMode==ToolMode.MarkPointMode){
                if(!onLine)Hit.collider.enabled = false;
                CommunicationEvents.TriggerEvent.Invoke(Hit);
            //    CommunicationEvents.SnapEvent.Invoke(Hit);
            }
                

          

        }
    }

   



 

}
