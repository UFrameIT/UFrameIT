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
        //We dont want to have this here anymore...
        //CommunicationEvents.RemoveFactEvent.AddListener(DeleteFact);

        NextEmpties.Add(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static bool findFact(Fact search, out Fact found)
    {
        foreach (Fact f in CommunicationEvents.Facts)
        {
            if (f.GetType() == search.GetType() && f.Equivalent(search))
            {
                found = f;
                return true;
            }
        }

        found = search;
        return false;
    }

    public static void addFact(int id, Fact fact)
    {
        CommunicationEvents.Facts.Insert(id, fact);
        //TODO (alt): insert in MMT if needed here/ on Invoke()
        //TODO: remove Inovkes() elsewhere
        CommunicationEvents.AddFactEvent.Invoke(fact);
    }

    public static Fact AddFactIfNotFound(int id, Fact fact, out bool exists)
    {
        if (exists = findFact(fact, out Fact res))
        {
            //TODO: del 'fact' in MMT (alt.: s.TODO in addFact)
            return res;
        }
        else
        {
            addFact(id, fact);
            return fact;
        }
    }

    public PointFact AddPointFact(RaycastHit hit, int id)
    {
        return (PointFact) AddFactIfNotFound(id, new PointFact(id, hit.point, hit.normal), out bool obsolete);
    }

    public PointFact AddPointFact(int id, Vector3 point, Vector3 normal)
    {
        return (PointFact) AddFactIfNotFound(id, new PointFact(id, point, normal), out bool obsolete);
    }

    public OnLineFact AddOnLineFact(int pid, int lid, int id)
    {
        return (OnLineFact)AddFactIfNotFound(id, new OnLineFact(id, pid, lid), out bool obsolete);
    }

    public LineFact AddLineFact(int pid1, int pid2, int id)
    {
        return (LineFact)AddFactIfNotFound(id, new LineFact(id, pid1, pid2), out bool obsolete);
    }

    public RayFact AddRayFact(int pid1, int pid2, int id)
    {
        RayFact rayFact = (RayFact)AddFactIfNotFound(id, new RayFact(id, pid1, pid2), out bool exists);
        if (exists)
            return rayFact;

        //Add all PointFacts on Ray as OnLineFacts
        PointFact rayP1 = (PointFact)Facts[rayFact.Pid1];
        int layerMask = LayerMask.GetMask("Point");
        RaycastHit[] hitsA = Physics.RaycastAll(rayP1.Point,  rayFact.Dir, Mathf.Infinity, layerMask);
        RaycastHit[] hitsB = Physics.RaycastAll(rayP1.Point, -rayFact.Dir, Mathf.Infinity, layerMask);

        void AddHitIfOnLine(RaycastHit hit)
        {
            if (Math3d.IsPointApproximatelyOnLine(rayP1.Point, rayFact.Dir, hit.transform.position))
            {
                AddOnLineFact(hit.transform.gameObject.GetComponent<FactObject>().Id, rayFact.Id, GetFirstEmptyID());
            }
        }

        foreach (RaycastHit hit in hitsA)
            AddHitIfOnLine(hit);

        foreach (RaycastHit hit in hitsB)
            AddHitIfOnLine(hit);

        return rayFact;
    }


    public AngleFact AddAngleFact(int pid1, int pid2, int pid3, int id)
    {
        return (AngleFact)AddFactIfNotFound(id, new AngleFact(id, pid1, pid2, pid3), out bool obsolete);
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
        NextEmpties.Sort();

        int id = NextEmpties[0];
        NextEmpties.RemoveAt(0);
        if (NextEmpties.Count == 0)
            NextEmpties.Add(id + 1);

        Debug.Log("place fact at " + id);

        return id;
    }
}
