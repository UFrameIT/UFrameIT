using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CommunicationEvents;
public class FactManager : MonoBehaviour
{
    public GameObject SmartMenu;
    private List<int> NextEmpties= new List<int>();

    //Variables for LineMode distinction
    public bool lineModeIsFirstPointSelected = false;
    public Fact lineModeFirstPointSelected = null;

    //Variables for AngleMode distinction
    public bool angleModeIsFirstLineSelected = false;
    public Fact angleModeFirstLineSelected = null;

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        CommunicationEvents.TriggerEvent.AddListener(OnHit);

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
       
        Facts.Insert(id, new PointFact
        {
       
            Id = id,
            Point = hit.point,
            Normal = hit.normal
        });

        return Facts.Find(x => x.Id == id) as PointFact;
    }

    LineFact AddLineFact(int pid1, int pid2, int id)
    {
       Facts.Insert(id, new LineFact
        {
            Id = id,
            Pid1 = pid1,
            Pid2 = pid2
        });

        return Facts.Find(x => x.Id == id) as LineFact;
    }

    AngleFact AddAngleFact(int pid1, int pid2, int pid3, int id)
    {
        Facts.Insert(id, new AngleFact
        {
            Id = id,
            Pid1 = pid1,
            Pid2 = pid2,
            Pid3 = pid3
        });

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
                //If CreateAngleMode is activated we want to have the ability to select Lines for the Angle
                //but we don't want to have the ability to select Points or Angles
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    if (gO.layer == LayerMask.NameToLayer("Point") || gO.layer == LayerMask.NameToLayer("Angle"))
                    {
                        gO.GetComponentInChildren<Collider>().enabled = false;
                    }
                    else if (gO.layer == LayerMask.NameToLayer("Line"))
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
                //Check if an existing Line was hit
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Line"))
                {
                    Fact tempFact = Facts[hit.transform.GetComponent<FactObject>().Id];

                    if (this.angleModeIsFirstLineSelected)
                    {
                        //Event for end of line-rendering in "ShinyThings"
                        CommunicationEvents.StopCurveDrawingEvent.Invoke(null);
                        //Create AngleFact
                        //Check if selected Lines are the same -> if true -> cancel
                        if (!(angleModeFirstLineSelected.Id == tempFact.Id))
                        {
                            //Check if selected Lines have a common Point = id2 for AngleFact
                            if (((LineFact)angleModeFirstLineSelected).Pid1 == ((LineFact)tempFact).Pid1)
                            {
                                CommunicationEvents.AddFactEvent.Invoke(this.AddAngleFact(((LineFact)angleModeFirstLineSelected).Pid2, ((LineFact)tempFact).Pid1, ((LineFact)tempFact).Pid2, GetFirstEmptyID()));
                            }
                            else if (((LineFact)angleModeFirstLineSelected).Pid1 == ((LineFact)tempFact).Pid2)
                            {
                                CommunicationEvents.AddFactEvent.Invoke(this.AddAngleFact(((LineFact)angleModeFirstLineSelected).Pid2, ((LineFact)tempFact).Pid2, ((LineFact)tempFact).Pid1, GetFirstEmptyID()));
                            }
                            else if (((LineFact)angleModeFirstLineSelected).Pid2 == ((LineFact)tempFact).Pid1)
                            {
                                CommunicationEvents.AddFactEvent.Invoke(this.AddAngleFact(((LineFact)angleModeFirstLineSelected).Pid1, ((LineFact)tempFact).Pid1, ((LineFact)tempFact).Pid2, GetFirstEmptyID()));
                            }
                            else if (((LineFact)angleModeFirstLineSelected).Pid2 == ((LineFact)tempFact).Pid2)
                            {
                                CommunicationEvents.AddFactEvent.Invoke(this.AddAngleFact(((LineFact)angleModeFirstLineSelected).Pid1, ((LineFact)tempFact).Pid2, ((LineFact)tempFact).Pid1, GetFirstEmptyID()));
                            }
                            else
                            {
                                //TODO: Hint that the selected Lines have no common point
                            }
                        }

                        this.angleModeIsFirstLineSelected = false;
                        this.angleModeFirstLineSelected = null;
                    }
                    else
                    {
                        //Activate CurveDrawing for preview
                        this.angleModeIsFirstLineSelected = true;
                        this.angleModeFirstLineSelected = tempFact;
                        //Event for start line-rendering in "ShinyThings"
                        CommunicationEvents.StartCurveDrawingEvent.Invoke(this.angleModeFirstLineSelected);
                    }
                }
                else
                {
                    //TODO: If Point was hit: Angle Drawing with Selecting 3 Points
                    if (this.angleModeIsFirstLineSelected)
                    {
                        //Deactivate CurveDrawing and first line selection
                        this.angleModeIsFirstLineSelected = false;
                        this.angleModeFirstLineSelected = null;
                        //Event for end of line-drawing in "ShinyThings"
                        CommunicationEvents.StopCurveDrawingEvent.Invoke(null);
                    }

                    //TODO: Hint that only a curve can be drawn between already existing lines
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
