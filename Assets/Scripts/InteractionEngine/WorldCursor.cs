using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;
using static GadgetManager;

public class WorldCursor : MonoBehaviour
{
    public RaycastHit Hit;
    // TODO experimentell for multiple hits
    public RaycastHit[] MultipleHits;

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

    public void setLayerMask(int layerMask)
    {
        this.layerMask = layerMask;
    }

    //void Update()
    //{
    //    Ray ray = useCamCurser ? new Ray(Cam.transform.position, Cam.transform.forward) : Cam.ScreenPointToRay(Input.mousePosition);

    //    this.Hit = new RaycastHit();
    //    transform.up = Cam.transform.forward;
    //    transform.position = ray.GetPoint(GlobalBehaviour.GadgetPhysicalDistance);

    //    int rayCastMask;
    //    if (Input.GetButton(this.deactivateSnapKey))
    //        rayCastMask = this.layerMask & ~this.snapLayerMask.value;
    //    else
    //        rayCastMask = this.layerMask;

    //    if (Physics.Raycast(ray, out Hit, MaxRange, rayCastMask)
    //        || (MaxRange <= GlobalBehaviour.GadgetPhysicalDistance 
    //        && Physics.Raycast(transform.position, Vector3.down, out Hit, GlobalBehaviour.GadgetPhysicalDistance, rayCastMask)))
    //    {
    //        if ((Hit.collider.transform.CompareTag("SnapZone") || Hit.collider.transform.CompareTag("Selectable")) 
    //            && !Input.GetButton(this.deactivateSnapKey))
    //        {
    //            if(Hit.collider.gameObject.layer == LayerMask.NameToLayer("Ray")
    //                || Hit.collider.gameObject.layer == LayerMask.NameToLayer("Line"))
    //            {
    //                var id = Hit.collider.gameObject.GetComponent<FactObject>().URI;
    //                AbstractLineFact lineFact = StageStatic.stage.factState[id] as AbstractLineFact;
    //                PointFact p1 =  StageStatic.stage.factState[lineFact.Pid1] as PointFact;

    //                Hit.point = Math3d.ProjectPointOnLine(p1.Point, lineFact.Dir, Hit.point);
    //            }
    //            else
    //            {
    //                Hit.point = Hit.collider.transform.position;
    //                Hit.normal = Vector3.up;
    //            }

    //            transform.position = Hit.point;
    //            transform.up = Hit.normal;

    //        }
    //        else
    //        {
    //            transform.position = Hit.point;
    //            transform.up = Hit.normal;
    //            transform.position += .01f * Hit.normal;
    //        }

    //        CheckMouseButtons();

    //    }
    //}


    // working currently to include multiple hits 
    // TODO 

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

        // in case we dont hit anything, just return
        if (!(Physics.Raycast(ray, out Hit, MaxRange, rayCastMask)
            || (MaxRange <= GlobalBehaviour.GadgetPhysicalDistance
            && Physics.Raycast(transform.position, Vector3.down, out Hit, GlobalBehaviour.GadgetPhysicalDistance, rayCastMask))))
            return;

        RaycastHit[] multipleHits = Physics.RaycastAll(ray, MaxRange, rayCastMask);
        if (multipleHits.Length == 0)
            multipleHits = Physics.RaycastAll(transform.position, Vector3.down, GlobalBehaviour.GadgetPhysicalDistance, rayCastMask);



        // sort multipleHits, so the first hit is still the closest 
        for (int i = 0; i < multipleHits.Length; i++)
        {
            int minIdx = i;
            float minValue = multipleHits[i].distance;

            for (int j = i; j < multipleHits.Length; j++)
            {
                if (multipleHits[j].distance < minValue)
                {
                    minIdx = j;
                    minValue = multipleHits[j].distance;
                }

            }

            RaycastHit buffer = multipleHits[minIdx];
            multipleHits[minIdx] = multipleHits[i];
            multipleHits[i] = buffer;

        }
            

        for (int i = 0; i < multipleHits.Length; i++)
        {
            // check whether we actually hit something 
            if (!((multipleHits[i].collider.transform.CompareTag("SnapZone") || multipleHits[i].collider.transform.CompareTag("Selectable"))
                && !Input.GetButton(this.deactivateSnapKey)))
                continue;

            if (multipleHits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ray")
                || multipleHits[i].collider.gameObject.layer == LayerMask.NameToLayer("Line"))
            {
                var id = multipleHits[i].collider.gameObject.GetComponent<FactObject>().URI;
                AbstractLineFact lineFact = StageStatic.stage.factState[id] as AbstractLineFact;
                PointFact p1 = StageStatic.stage.factState[lineFact.Pid1] as PointFact;

                multipleHits[i].point = Math3d.ProjectPointOnLine(p1.Point, lineFact.Dir, multipleHits[i].point);
            }
            else if (multipleHits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ring"))
            {
                #region Ring
                Debug.Log("PRINT");
                var id = multipleHits[i].transform.GetComponent<FactObject>().URI;
                CircleFact circleFact = StageStatic.stage.factState[id] as CircleFact;
                PointFact middlePoint = StageStatic.stage.factState[circleFact.Pid1] as PointFact;
                var normal = circleFact.normal;
                        


                // generate circle
                int pointCount = multipleHits[i].transform.GetComponentInParent<TorusGenerator>().ringSegmentCount;
                Vector3[] circle = new Vector3[pointCount];
                float slice = (2f * Mathf.PI) / pointCount;
                for (int j = 0; j < pointCount; j++)
                {
                    // generate possible snappoints one the "corners" of the torus mesh
                    float angle = j * slice;
                    circle[j] = new Vector3(circleFact.radius * Mathf.Sin(angle), 0, circleFact.radius * Mathf.Cos(angle)) + middlePoint.Point;

                    // rotate snappoint according to circle normal
                    circle[j] = Quaternion.LookRotation(new Vector3(-normal.z, 0, normal.x), normal) * circle[j];   
                }

                // get closest cornerPoint
                multipleHits[i].point = circle.OrderBy(p => Vector3.Distance(p, multipleHits[i].point)).First();
                #endregion Ring
            }
            else
            {
                multipleHits[i].point = multipleHits[i].collider.transform.position;
                multipleHits[i].normal = Vector3.up;
            }

            // checking for 2 lines intersection point
            if (!((Mathf.Abs(multipleHits[i].distance - multipleHits[0].distance) < 0.03)
                && (multipleHits.Length > 1)
                && (Mathf.Abs(multipleHits[1].distance - multipleHits[0].distance) < 0.03)))
                continue;
            // we probably have two objects intersecting 

            
            // check for line x line intersection and if they actually intersect adjust the points coordinates :)
            if (multipleHits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ray")
                && multipleHits[0].collider.gameObject.layer == LayerMask.NameToLayer("Ray"))
            {

                // case for two intersecting rays 
                var idLine0 = multipleHits[0].collider.gameObject.GetComponent<FactObject>().URI;
                var id = multipleHits[i].collider.gameObject.GetComponent<FactObject>().URI;

                // get the two corresponding line facts
                AbstractLineFact lineFactLine0 = StageStatic.stage.factState[idLine0] as AbstractLineFact;
                AbstractLineFact lineFact = StageStatic.stage.factState[id] as AbstractLineFact;

                // get a point on the line 
                PointFact p1Line0 = StageStatic.stage.factState[lineFactLine0.Pid1] as PointFact;
                PointFact p1 = StageStatic.stage.factState[lineFact.Pid1] as PointFact;

                // get the intersection point and if it actually intersects set it
                Vector3 intersectionPoint = Vector3.zero;
 
                if (Math3d.LineLineIntersection(out intersectionPoint, p1Line0.Point, lineFactLine0.Dir, p1.Point, lineFact.Dir))
                    multipleHits[i].point = intersectionPoint;


            }
            //check for other types of intersection. Future Work






        }

        transform.position = multipleHits[0].point;
        transform.up = multipleHits[0].normal;
        if (!((multipleHits[0].collider.transform.CompareTag("SnapZone") || multipleHits[0].collider.transform.CompareTag("Selectable"))
              && !Input.GetButton(this.deactivateSnapKey)))
            transform.position += .01f * multipleHits[0].normal;






        this.MultipleHits = multipleHits;
        CheckMouseButtons();



    }





    //Check if left Mouse-Button was pressed and handle it
    void CheckMouseButtons()
    {
        //TODO massively edit for the multiple hits. Right now it only checks the first hit
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() //this prevents rays from shooting through ui
                || Hit.transform.gameObject.layer == LayerMask.NameToLayer("Water")) // not allowed to meassure on water
                return;

            CommunicationEvents.TriggerEvent.Invoke(MultipleHits);
        }
    }

}
