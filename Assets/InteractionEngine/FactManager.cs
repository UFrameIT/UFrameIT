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

    //Solving game parameters
    public GameObject snapZoneTop;
    public GameObject snapZoneBottom;
    public static Vector3 solutionVector;
    public static bool solved = false;

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);

        //  CommunicationEvents.SnapEvent.AddListener(Rocket);

        //We dont want to have this here anymore...
        //CommunicationEvents.RemoveFactEvent.AddListener(DeleteFact);

        NextEmpties.Add(0);

        //Calculate Solution-Vector
        solutionVector = snapZoneTop.transform.position - snapZoneBottom.transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TODO: change the return find....
    public PointFact AddPointFact(RaycastHit hit, int id)
    {
        Facts.Insert(id, new PointFact(id, hit.point, hit.normal));
        return Facts.Find(x => x.Id == id) as PointFact;
    }

    public LineFact AddLineFact(int pid1, int pid2, int id)
    {
        Facts.Insert(id, new LineFact(id, pid1, pid2));

        return Facts.Find(x => x.Id == id) as LineFact;
    }
    public RayFact AddRayFact(int pid1, int pid2, int id)
    {
        Facts.Insert(id, new RayFact(id, pid1, pid2));

        var oLid = GetFirstEmptyID();
        Facts.Insert(oLid, new OnLineFact(oLid, pid1, id));
        oLid = GetFirstEmptyID();
        Facts.Insert(oLid, new OnLineFact(oLid, pid2, id));

        var p1 = Facts.Find(x => x.Id == pid1);
        var p2 = Facts.Find(x => x.Id == pid2);

        Vector3 dir = p2.Representation.transform.position - p1.Representation.transform.position;

    

        // Bit shift the index of the layer Point to get a bit mask
        int layerMask = 1 << LayerMask.NameToLayer("Point");
        // This casts rays only against colliders in layer 8


        RaycastHit[] hits;
        hits = Physics.RaycastAll(p1.Representation.transform.position - dir * 1000, dir, Mathf.Infinity, layerMask);

        Debug.Log(hits.Length + " hits");
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            bool exists = false;

            foreach (Fact fact in Facts)
            {
                if (typeof(OnLineFact).IsInstanceOfType(fact))
                {
                    OnLineFact oLFact = (OnLineFact)fact;
                    if ((oLFact.Lid == id && oLFact.Pid == hit.transform.gameObject.GetComponent<FactObject>().Id))
                    {
                        exists = true;
                        break;
                    }
                }
            }


            if (!exists)
            {
                oLid = GetFirstEmptyID();
                var olF = new OnLineFact(oLid, hit.transform.gameObject.GetComponent<FactObject>().Id, id);
                Facts.Insert(oLid, olF);
            }


        }
        
        return Facts.Find(x => x.Id == id) as RayFact;
    }


    public AngleFact AddAngleFact(int pid1, int pid2, int pid3, int id)
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

    public Boolean factAlreadyExists(int[] ids)
    {
        switch (ActiveToolMode)
        {
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
    }

    public static Boolean gameSolved() {

        Vector3 tempDir1 = new Vector3(0, 0, 0);
        Vector3 tempDir2 = new Vector3(0, 0, 0);

        if (solved == true)
            return true;
        else {
            //Look for solutionFact in global factList
            foreach (Fact fact in Facts)
            {
                if (typeof(LineFact).IsInstanceOfType(fact))
                {
                    tempDir1 = ((PointFact)Facts.Find(x => x.Id == ((LineFact)fact).Pid1)).Point - ((PointFact)Facts.Find(x => x.Id == ((LineFact)fact).Pid2)).Point;
                    tempDir2 = ((PointFact)Facts.Find(x => x.Id == ((LineFact)fact).Pid2)).Point - ((PointFact)Facts.Find(x => x.Id == ((LineFact)fact).Pid1)).Point;
                    if (solutionVector == tempDir1 || solutionVector == tempDir2)
                    {
                        solved = true;
                        return true;
                    }
                }
            }
            return false;
        }
    }
    /*
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
    }*/

    public void OnToolModeChanged(ToolMode ActiveToolMode)
    {

        //TODO: instead of enabling/disabling colliders we want to change the raycast mask
        switch (ActiveToolMode)
        {
            case ToolMode.MarkPointMode:
                //If MarkPointMode is activated we want to have the ability to mark the point
                //everywhere, independent of already existing facts
                foreach (Fact fact in Facts)
                {
                
                    GameObject gO = fact.Representation;
                    if (gO == null) continue;
                    if ((gO.layer == LayerMask.NameToLayer("Ray")))
                            gO.GetComponentInChildren<Collider>().enabled = true;
                }
                break;

            case ToolMode.CreateRayMode:
                //same as for line mode atm

            case ToolMode.CreateLineMode:
                //If CreateLineMode is activated we want to have the ability to select points for the Line
                //but we don't want to have the ability to select Lines or Angles
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    if (gO == null) continue;
                    if (gO.layer == LayerMask.NameToLayer("Line") || gO.layer == LayerMask.NameToLayer("Angle")|| gO.layer == LayerMask.NameToLayer("Ray"))
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
                    if (gO == null) continue;
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
                /*
            case ToolMode.DeleteMode:
                //If DeleteMode is activated we want to have the ability to delete every Fact
                //independent of the concrete type of fact
                foreach (Fact fact in Facts)
                {
                    GameObject gO = fact.Representation;
                    gO.GetComponentInChildren<Collider>().enabled = true;
                }
                break;*/
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

}
