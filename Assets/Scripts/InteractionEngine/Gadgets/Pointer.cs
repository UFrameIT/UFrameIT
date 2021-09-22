using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Pointer : Gadget
{
    new void Awake()
    {
        base.Awake();
        this.UiName = "Point Mode";
        if (MaxRange == 0)
            MaxRange = GlobalBehaviour.GadgetLaserDistance;
    }

    public override void OnHit(RaycastHit hit)
    {

        if (!this.isActiveAndEnabled) return;
        var pid = FactManager.AddPointFact(hit).Id;

        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ray"))
        {
            FactManager.AddOnLineFact(pid, hit.transform.GetComponent<FactObject>().URI, true);
        }
    }
  
}
