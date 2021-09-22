using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static CommunicationEvents;

public class FactManager : MonoBehaviour
{
    //TODO! communicate success/ failure
    public static Fact AddFactIfNotFound(Fact fact, out bool exists, bool samestep)
    {
        return StageStatic.stage.factState[StageStatic.stage.factState.Add(fact, out exists, samestep)];
    }

    public PointFact AddPointFact(RaycastHit hit, bool samestep = false)
    {
        return (PointFact) AddFactIfNotFound(new PointFact(hit.point, hit.normal, StageStatic.stage.factState), out _, samestep);
    }

    public PointFact AddPointFact(Vector3 point, Vector3 normal, bool samestep = false)
    {
        return (PointFact) AddFactIfNotFound(new PointFact(point, normal, StageStatic.stage.factState), out _, samestep);
    }

    public OnLineFact AddOnLineFact(string pid, string lid, bool samestep = false)
    {
        return (OnLineFact)AddFactIfNotFound(new OnLineFact(pid, lid, StageStatic.stage.factState), out _, samestep);
    }

    public LineFact AddLineFact(string pid1, string pid2, bool samestep = false)
    {
        return (LineFact)AddFactIfNotFound(new LineFact(pid1, pid2, StageStatic.stage.factState), out _, samestep);
    }

    public RayFact AddRayFact(string pid1, string pid2, bool samestep = false)
    {
        RayFact rayFact = (RayFact)AddFactIfNotFound(new RayFact(pid1, pid2, StageStatic.stage.factState), out bool exists, samestep);
        if (exists)
            return rayFact;

        //Add all PointFacts on Ray as OnLineFacts
        PointFact rayP1 = (PointFact)StageStatic.stage.factState[rayFact.Pid1];
        PointFact rayP2 = (PointFact)StageStatic.stage.factState[rayFact.Pid2];
        int layerMask = LayerMask.GetMask("Point");
        RaycastHit[] hitsA = Physics.RaycastAll(rayP1.Point,  rayFact.Dir, Mathf.Infinity, layerMask);
        RaycastHit[] hitsB = Physics.RaycastAll(rayP2.Point, -rayFact.Dir, Mathf.Infinity, layerMask);

        void AddHitIfOnLine(RaycastHit hit)
        {
            if (Math3d.IsPointApproximatelyOnLine(rayP1.Point, rayFact.Dir, hit.transform.position))
            {
                AddOnLineFact(hit.transform.gameObject.GetComponent<FactObject>().URI, rayFact.Id, true);
            }
        }

        foreach (RaycastHit hit in hitsA)
            AddHitIfOnLine(hit);

        foreach (RaycastHit hit in hitsB)
            AddHitIfOnLine(hit);

        // for good measure
        AddOnLineFact(rayFact.Pid1, rayFact.Id, true);
        AddOnLineFact(rayFact.Pid2, rayFact.Id, true);

        return rayFact;
    }

    public AngleFact AddAngleFact(string pid1, string pid2, string pid3, bool samestep = false)
    {
        return (AngleFact)AddFactIfNotFound(new AngleFact(pid1, pid2, pid3, StageStatic.stage.factState), out _, samestep);
    }

}
