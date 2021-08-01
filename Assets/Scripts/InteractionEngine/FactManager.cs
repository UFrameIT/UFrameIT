using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CommunicationEvents;

public class FactManager : MonoBehaviour
{
    private List<int> NextEmpties = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        NextEmpties.Add(0);
    }

    //TODO! communicate success
    public static Fact AddFactIfNotFound(Fact fact, out bool exists, bool samestep)
    {
        return Facts[Facts.Add(fact, out exists, samestep)];
    }

    public PointFact AddPointFact(RaycastHit hit, int id, bool samestep = false)
    {
        return (PointFact) AddFactIfNotFound(new PointFact(id, hit.point, hit.normal), out bool obsolete, samestep);
    }

    public PointFact AddPointFact(int id, Vector3 point, Vector3 normal, bool samestep = false)
    {
        return (PointFact) AddFactIfNotFound(new PointFact(id, point, normal), out bool obsolete, samestep);
    }

    public OnLineFact AddOnLineFact(int pid, int lid, int id, bool samestep = false)
    {
        return (OnLineFact)AddFactIfNotFound(new OnLineFact(id, pid, lid), out bool obsolete, samestep);
    }

    public LineFact AddLineFact(int pid1, int pid2, int id, bool samestep = false)
    {
        return (LineFact)AddFactIfNotFound(new LineFact(id, pid1, pid2), out bool obsolete, samestep);
    }

    public RayFact AddRayFact(int pid1, int pid2, int id, bool samestep = false)
    {
        RayFact rayFact = (RayFact)AddFactIfNotFound(new RayFact(id, pid1, pid2), out bool exists, samestep);
        if (exists)
            return rayFact;

        //Add all PointFacts on Ray as OnLineFacts
        PointFact rayP1 = (PointFact)Facts[rayFact.Pid1];
        PointFact rayP2 = (PointFact)Facts[rayFact.Pid2];
        int layerMask = LayerMask.GetMask("Point");
        RaycastHit[] hitsA = Physics.RaycastAll(rayP1.Point,  rayFact.Dir, Mathf.Infinity, layerMask);
        RaycastHit[] hitsB = Physics.RaycastAll(rayP2.Point, -rayFact.Dir, Mathf.Infinity, layerMask);

        void AddHitIfOnLine(RaycastHit hit)
        {
            if (Math3d.IsPointApproximatelyOnLine(rayP1.Point, rayFact.Dir, hit.transform.position))
            {
                AddOnLineFact(hit.transform.gameObject.GetComponent<FactObject>().Id, rayFact.Id, GetFirstEmptyID(), true);
            }
        }

        foreach (RaycastHit hit in hitsA)
            AddHitIfOnLine(hit);

        foreach (RaycastHit hit in hitsB)
            AddHitIfOnLine(hit);

        // for good measure
        AddOnLineFact(rayFact.Pid1, rayFact.Id, GetFirstEmptyID(), true);
        AddOnLineFact(rayFact.Pid2, rayFact.Id, GetFirstEmptyID(), true);

        return rayFact;
    }


    public AngleFact AddAngleFact(int pid1, int pid2, int pid3, int id, bool samestep = false)
    {
        return (AngleFact)AddFactIfNotFound(new AngleFact(id, pid1, pid2, pid3), out bool obsolete, samestep);
    }

    public int GetFirstEmptyID()
    {
        NextEmpties.Sort();

        int id = NextEmpties[0];
        NextEmpties.RemoveAt(0);
        if (NextEmpties.Count == 0)
            NextEmpties.Add(id + 1);

        Debug.Log("place fact at " + id);

        return id;
    }
}
