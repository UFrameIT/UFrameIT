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



    // Start is called before the first frame update
    void Start()
    {

        //  CommunicationEvents.SnapEvent.AddListener(Rocket);

        //We dont want to have this here anymore...
        //CommunicationEvents.RemoveFactEvent.AddListener(DeleteFact);

        NextEmpties.Add(0);

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
        if (GadgetManager.activeGadget.GetType() == typeof(Tape)) {
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
        }
        else if (GadgetManager.activeGadget.GetType() == typeof(AngleTool))
        {
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
        }
        else
            return false;
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
    */

}
