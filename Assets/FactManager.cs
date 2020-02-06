using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CommunicationEvents;
public class FactManager : MonoBehaviour
{
    public GameObject SmartMenu;
    private List<int> NextEmpties = new List<int>();

    //Variables for LineMode distinction
    public bool lineModeIsFirstPointSelected = false;
    public Fact lineModeFirstPointSelected = null;

    //Variables for AngleMode distinction
    public bool angleModeIsFirstPointSelected = false;
    public Fact angleModeFirstPointSelected = null;
    public bool angleModeIsSecondPointSelected = false;
    public Fact angleModeSecondPointSelected = null;

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
        //  CommunicationEvents.SnapEvent.AddListener(Rocket);

        //We dont want to have this here anymore...
        //CommunicationEvents.RemoveFactEvent.AddListener(DeleteFact);

        NextEmpties.Add(0);

    }

    // Update is called once per frame
    void Update()
    {

    }

    PointFact AddPointFact(RaycastHit hit, int id)
    {
        Facts.Insert(id, new PointFact(id, hit.point, hit.normal));
        return Facts.Find(x => x.Id == id) as PointFact;
    }

    LineFact AddLineFact(int pid1, int pid2, int id)
    {
        Facts.Insert(id, new LineFact(id, pid1, pid2));

        return Facts.Find(x => x.Id == id) as LineFact;
    }

    AngleFact AddAngleFact(int pid1, int pid2, int pid3, int id)
    {
        Facts.Insert(id, new AngleFact(id, pid1, pid2, pid3));

        return Facts.Find(x => x.Id == id) as AngleFact;
    }

    public void DeleteFact(Fact fact)
    {
        if (Facts.Contains(fact)) {
            NextEmpties.Add(fact.Id);
            //Facts.RemoveAt(fact.Id);
            Facts.Remove(Facts.Find(x => x.Id == fact.Id));
            CommunicationEvents.RemoveFactEvent.Invoke(fact);
        }
    }

    public int GetFirstEmptyID()
    {

        /* for (int i = 0; i < Facts.Length; ++i)
         {
             if (Facts[i] == "")
                 return i;
         }
         return Facts.Length - 1;*/
        NextEmpties.Sort();

        int id = NextEmpties[0];
        NextEmpties.RemoveAt(0);
        if (NextEmpties.Count == 0)
            NextEmpties.Add(id + 1);

        Debug.Log("place fact at " + id);

        return id;


    }

    public void OnToolModeChanged(ToolMode ActiveToolMode)
    {
        //We need to do this somehwere...
        CommunicationEvents.ActiveToolMode = ActiveToolMode;

        switch (ActiveToolMode)
        {
            case ToolMode.MarkPointMode:
                //If MarkPointMode is activated we want to have the ability to mark the point
                //everywhere, independent of already existing facts
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    gO.GetComponentInChildren<Collider>().enabled = false;
                }
                break;
            case ToolMode.CreateLineMode:
                //If CreateLineMode is activated we want to have the ability to select points for the Line
                //but we don't want to have the ability to select Lines or Angles
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    if (gO.layer == LayerMask.NameToLayer("Line") || gO.layer == LayerMask.NameToLayer("Angle"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (gO.layer == LayerMask.NameToLayer("Point"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.CreateAngleMode:
                //If CreateAngleMode is activated we want to have the ability to select Points for the Angle
                //but we don't want to have the ability to select Lines or Angles
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    if (gO.layer == LayerMask.NameToLayer("Line") || gO.layer == LayerMask.NameToLayer("Angle"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (gO.layer == LayerMask.NameToLayer("Point"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = true;
                    }
                }
                break;
            case ToolMode.DeleteMode:
                //If DeleteMode is activated we want to have the ability to delete every Fact
                //independent of the concrete type of fact
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    gO.GetComponentInChildren<Collider>().enabled = true;
                }
                break;
            case ToolMode.ExtraMode:
                /*foreach (Fact fact in Facts)
                {

                }
                */
                break;
        }
        //Stop PreviewEvents in ShineThings on ToolModeChange
        CommunicationEvents.StopPreviewsEvent.Invoke(null);
    }





    //automatic 90 degree angle construction
    public void Rocket(RaycastHit hit)
    {

        int idA, idB, idC;

        //usual point
        idA = this.GetFirstEmptyID();
        CommunicationEvents.AddFactEvent.Invoke(this.AddPointFact(hit, idA));

        //second point
        idB = this.GetFirstEmptyID();
        var shiftedHit = hit;
        var playerPos = Camera.main.transform.position;
        playerPos.y = hit.point.y;
        shiftedHit.point = playerPos;
        CommunicationEvents.AddFactEvent.Invoke(this.AddPointFact(shiftedHit, idB));

        //third point with unknown height
        idC = this.GetFirstEmptyID();
        var skyHit = hit;
        skyHit.point += Vector3.up * 20;
        CommunicationEvents.AddFactEvent.Invoke(this.AddPointFact(skyHit, idC));

        //lines
        CommunicationEvents.AddFactEvent.Invoke(this.AddLineFact(idA, idB, this.GetFirstEmptyID()));
        //lines
        CommunicationEvents.AddFactEvent.Invoke(this.AddLineFact(idA, idC, this.GetFirstEmptyID()));

        //90degree angle
        CommunicationEvents.AddFactEvent.Invoke(this.AddAngleFact(idB, idA, idC, GetFirstEmptyID()));
    }

    //Creating 90-degree Angles
    public void SmallRocket(RaycastHit hit, int idA)
    {
        //enable collider to measure angle to the treetop



        int idB = this.GetFirstEmptyID();
        CommunicationEvents.AddFactEvent.Invoke(this.AddPointFact(hit, idB));
        Facts[idB].Representation.GetComponentInChildren<Collider>().enabled = true;

        //third point with unknown height
        int idC = this.GetFirstEmptyID();
        var skyHit = hit;
        skyHit.point = (Facts[idA] as PointFact).Point + Vector3.up * 20;
        CommunicationEvents.AddFactEvent.Invoke(this.AddPointFact(skyHit, idC));

        //lines
        CommunicationEvents.AddFactEvent.Invoke(this.AddLineFact(idA, idB, this.GetFirstEmptyID()));
        //lines
        CommunicationEvents.AddFactEvent.Invoke(this.AddLineFact(idA, idC, this.GetFirstEmptyID()));

        //90degree angle
        CommunicationEvents.AddFactEvent.Invoke(this.AddAngleFact(idB, idA, idC, GetFirstEmptyID()));
    }

    public Boolean factAlreadyExists(int[] ids) {
        switch (ActiveToolMode) {
            case ToolMode.CreateLineMode:
                foreach (Fact fact in Facts)
                {
                    if (typeof(LineFact).IsInstanceOfType(fact))
                    {
                        LineFact line = (LineFact)fact;
                        if (line.Pid1 == ids[0] && line.Pid2 == ids[1])
                        {
                            return true;
                        }
                    }
                }
                return false;
            case ToolMode.CreateAngleMode:
                foreach (Fact fact in Facts)
                {
                    if (typeof(AngleFact).IsInstanceOfType(fact))
                    {
                        AngleFact angle = (AngleFact)fact;
                        if (angle.Pid1 == ids[0] && angle.Pid2 == ids[1] && angle.Pid3 == ids[2])
                        {
                            return true;
                        }
                    }
                }
                return false;
            default:
                return false;
        }
        return false;
    }

    public void OnHit(RaycastHit hit)
    {
        
        switch (ActiveToolMode)
        {
            //If Left-Mouse-Button was pressed in MarkPointMode
            case ToolMode.MarkPointMode:
                CommunicationEvents.AddFactEvent.Invoke(this.AddPointFact(hit, this.GetFirstEmptyID()));
                break;
            //If Left-Mouse-Button was pressed in CreateLineMode
            case ToolMode.CreateLineMode:
                //Check if an existing Point was hit
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
                {
                    Fact tempFact = Facts[hit.transform.GetComponent<FactObject>().Id];

                    if (this.lineModeIsFirstPointSelected)
                    {
                        //Event for end of line-drawing in "ShinyThings"
                        CommunicationEvents.StopLineDrawingEvent.Invoke(null);
                        //Create LineFact
                        //Check if exactle the same line/distance already exists
                        if(!factAlreadyExists(new int[] { this.lineModeFirstPointSelected.Id, tempFact.Id }))
                            CommunicationEvents.AddFactEvent.Invoke(this.AddLineFact(this.lineModeFirstPointSelected.Id, tempFact.Id, this.GetFirstEmptyID()));
                        this.lineModeIsFirstPointSelected = false;
                        this.lineModeFirstPointSelected = null;
                    }
                    else
                    {
                        //Activate LineDrawing for preview
                        this.lineModeIsFirstPointSelected = true;
                        this.lineModeFirstPointSelected = tempFact;
                        //Event for start line-drawing in "ShinyThings"
                        CommunicationEvents.StartLineDrawingEvent.Invoke(this.lineModeFirstPointSelected);
                    }
                }
                //if we want to spawn a new point
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (this.lineModeIsFirstPointSelected)
                    {


                        CommunicationEvents.StopLineDrawingEvent.Invoke(null);


                        SmallRocket(hit, this.lineModeFirstPointSelected.Id);


                        this.lineModeIsFirstPointSelected = false;
                        this.lineModeFirstPointSelected = null;
                    }
                }

                //if we hit the top snap zone
                else if (hit.transform.gameObject.tag=="SnapZone")
                {
                    if (this.lineModeIsFirstPointSelected)
                    {

                        RaycastHit downHit;

                        if (Physics.Raycast(hit.transform.gameObject.transform.position-Vector3.down*2,Vector3.down, out downHit))
                        {
                            int idA = downHit.transform.gameObject.GetComponent<FactObject>().Id;
                            int idB = this.lineModeFirstPointSelected.Id;
                            int idC = GetFirstEmptyID();
                            CommunicationEvents.AddFactEvent.Invoke(this.AddPointFact(hit, idC));
                            //Event for end of line-drawing in "ShinyThings"
                            CommunicationEvents.StopLineDrawingEvent.Invoke(null);
                            //Create LineFact
                            CommunicationEvents.AddFactEvent.Invoke(this.AddAngleFact(idA, idB, idC, GetFirstEmptyID()));
                            this.lineModeIsFirstPointSelected = false;
                            this.lineModeFirstPointSelected = null;
                        }
                    }
                }

                //If no Point was hit
                else
                {
                    if (this.lineModeIsFirstPointSelected)
                    {
                        //Deactivate LineDrawing and first point selection
                        this.lineModeIsFirstPointSelected = false;
                        this.lineModeFirstPointSelected = null;
                        //Event for end of line-drawing in "ShinyThings"
                        CommunicationEvents.StopLineDrawingEvent.Invoke(null);
                    }

                    //TODO: Hint that only a line can be drawn between already existing points
                }
                break;
            //If Left-Mouse-Button was pressed in CreateAngleMode
            case ToolMode.CreateAngleMode:
                //Check if an existing Point was hit
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point"))
                {
                    Fact tempFact = Facts[hit.transform.GetComponent<FactObject>().Id];

                    //If two points were already selected and now the third point got selected
                    if (this.angleModeIsFirstPointSelected && this.angleModeIsSecondPointSelected)
                    {
                        //Event for end of curve-drawing in "ShinyThings"
                        CommunicationEvents.StopCurveDrawingEvent.Invoke(null);
                        //Create AngleFact
                        //Check if new Point is equal to one of the previous points -> if true -> cancel
                        if (!(angleModeFirstPointSelected.Id == tempFact.Id || angleModeSecondPointSelected.Id == tempFact.Id))
                        {
                            //Check if exactly the same angle already exists
                            if (!factAlreadyExists(new int[] { ((PointFact)angleModeFirstPointSelected).Id, ((PointFact)angleModeSecondPointSelected).Id, ((PointFact)tempFact).Id }))
                                CommunicationEvents.AddFactEvent.Invoke(this.AddAngleFact(((PointFact)angleModeFirstPointSelected).Id, ((PointFact)angleModeSecondPointSelected).Id, ((PointFact)tempFact).Id, GetFirstEmptyID()));
                        }

                        this.angleModeIsFirstPointSelected = false;
                        this.angleModeFirstPointSelected = null;
                        this.angleModeIsSecondPointSelected = false;
                        this.angleModeSecondPointSelected = null;
                    }
                    //If only one point was already selected
                    else if (this.angleModeIsFirstPointSelected && !this.angleModeIsSecondPointSelected) {
                        //Check if the 2 selected points are the same: If not
                        if (this.angleModeFirstPointSelected.Id != tempFact.Id)
                        {
                            this.angleModeIsSecondPointSelected = true;
                            this.angleModeSecondPointSelected = tempFact;

                            //Event for start of curve-drawing in "ShinyThings"
                            //Create new LineFact with the 2 points
                            LineFact tempLineFact = new LineFact();
                            tempLineFact.Pid1 = this.angleModeFirstPointSelected.Id;
                            tempLineFact.Pid2 = this.angleModeSecondPointSelected.Id;
                            CommunicationEvents.StartCurveDrawingEvent.Invoke(tempLineFact);
                        }
                        else {
                            this.angleModeFirstPointSelected = null;
                            this.angleModeIsFirstPointSelected = false;
                        }
                    }
                    //If no point was selected before
                    else
                    {
                        //Save the first point selected
                        this.angleModeIsFirstPointSelected = true;
                        this.angleModeFirstPointSelected = tempFact;
                    }
                }
                //No point was hit
                else
                {
                    if (this.angleModeIsFirstPointSelected && this.angleModeIsSecondPointSelected)
                    {
                        //Event for end of curve-drawing in "ShinyThings"
                        CommunicationEvents.StopCurveDrawingEvent.Invoke(null);
                    }

                    //Reset Angle-Preview-Attributes
                    this.angleModeIsFirstPointSelected = false;
                    this.angleModeFirstPointSelected = null;
                    this.angleModeIsSecondPointSelected = false;
                    this.angleModeSecondPointSelected = null;

                    //TODO: Hint that only an angle can be created between 3 already existing points
                }
                break;
            //If Left-Mouse-Button was pressed in DeleteMode
            case ToolMode.DeleteMode:
                //Search for the Fact that was hit
                //If the hit GameObject was a Point/Line/Angle
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Line") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Angle"))
                {
                    //Search for the suitable fact from the List
                    this.DeleteFact(Facts.Find(x => x.Id == hit.transform.GetComponent<FactObject>().Id));
                }
                break;
            //If Left-Mouse-Button was pressed in ExtraMode
            case ToolMode.ExtraMode:
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point")) {
                    var menu = GameObject.Instantiate(SmartMenu);
                    menu.GetComponent<SmartMenu>().FactManager = this;
                    menu.GetComponent<Canvas>().worldCamera = Camera.main;
                    menu.transform.SetParent(hit.transform);
                    menu.transform.localPosition = Vector3.up - Camera.main.transform.forward;
                }
                else
                {
                    PointFact fact = AddPointFact(hit, GetFirstEmptyID());
                    CommunicationEvents.AddFactEvent.Invoke(fact);
                }
                break;

        }
    }
}
