using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Pointer : Gadget
{

    public override void OnHit(RaycastHit hit)
    {
        if (!this.isActiveAndEnabled) return;
        var pid = FactManager.GetFirstEmptyID();
        CommunicationEvents.AddFactEvent.Invoke(FactManager.AddPointFact(hit, pid));
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ray"))
        {

            var oLid = FactManager.GetFirstEmptyID();
            Facts.Insert(oLid, new OnLineFact(oLid, pid, hit.transform.GetComponent<FactObject>().Id));
        }
    }
  
}
