using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CommunicationEvents;
public class FactManager : MonoBehaviour
{
    public GameObject SmartMenu;
    private Stack<int> NextEmptyStack = new Stack<int>();

    //Variable for LineMode distinction
    public bool lineModeFirstPointSelected = false;
    public Fact firstPointSelected = null;

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        CommunicationEvents.TriggerEvent.AddListener(OnHit);

        //We also need the listener here at the moment so we can react to UI delete events in ExtraMode -> Delete-Button
        CommunicationEvents.RemoveFactEvent.AddListener(DeleteFact);

        NextEmptyStack.Push(0);
  
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

        return Facts[id] as PointFact;
    }

    LineFact AddLineFact(int pid1, int pid2, int id)
    {
       Facts.Insert(id, new LineFact
        {
            Id = id,
            Pid1 = pid1,
            Pid2 = pid2
        });

        return Facts[id] as LineFact;
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

        return Facts[id] as AngleFact;
    }

    void DeleteFact(Fact fact)
    {
        if (Facts.Contains(fact)) {
            NextEmptyStack.Push(fact.Id);
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

        int id = NextEmptyStack.Pop();
        if (NextEmptyStack.Count == 0)
            NextEmptyStack.Push(id + 1);

     
        return id;


    }

    public void OnToolModeChanged(ToolMode ActiveToolMode)
    {

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

                    if (this.lineModeFirstPointSelected)
                    {
                        //Event for end of line-rendering in "ShinyThings"
                        CommunicationEvents.StopLineRendererEvent.Invoke(null);
                        //Create LineFact
                        CommunicationEvents.AddFactEvent.Invoke(this.AddLineFact(this.firstPointSelected.Id, tempFact.Id, this.GetFirstEmptyID()));
                        this.lineModeFirstPointSelected = false;
                        this.firstPointSelected = null;
                    }
                    else {
                        //Activate LineRenderer for preview
                        this.lineModeFirstPointSelected = true;
                        this.firstPointSelected = tempFact;
                        //Event for start line-rendering in "ShinyThings"
                        CommunicationEvents.StartLineRendererEvent.Invoke(this.firstPointSelected);
                    }
                }
                //If no Point was hit
                else {
                    if (this.lineModeFirstPointSelected)
                    {
                        //Deactivate LineRendering and first point selection
                        this.lineModeFirstPointSelected = false;
                        this.firstPointSelected = null;
                        //Event for end of line-rendering in "ShinyThings"
                        CommunicationEvents.StopLineRendererEvent.Invoke(null);
                    }

                    //TODO: Hint that only a line can be drawn between already existing points
                }
                break;
            //If Left-Mouse-Button was pressed in CreateAngleMode
            case ToolMode.CreateAngleMode:
                break;
            //If Left-Mouse-Button was pressed in DeleteMode
            case ToolMode.DeleteMode:
                //Search for the Fact that was hit
                //If the hit GameObject was a Point/Line/Angle
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Line") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Angle")){
                    //Search for the suitable fact from the List
                    this.DeleteFact(Facts.Find(x => x.Id == hit.transform.GetComponent<FactObject>().Id));
                }
                break;
            //If Left-Mouse-Button was pressed in ExtraMode
            case ToolMode.ExtraMode:
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Point")) {
                    var menu = GameObject.Instantiate(SmartMenu);
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
