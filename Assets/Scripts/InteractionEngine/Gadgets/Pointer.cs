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

    public override void OnHit(RaycastHit[] hit)
    {

        if (!this.isActiveAndEnabled) return;
        var pid = FactManager.AddPointFact(hit[0]).Id;
        for (int i= 0; i < hit.Length; i++)
        {
            // sort out hits that where too far away from the initial hit
            if (Mathf.Abs(hit[i].distance - hit[0].distance) > 0.03)
                break;

            if (hit[i].transform.gameObject.layer == LayerMask.NameToLayer("Ray"))
            {
                FactManager.AddOnLineFact(pid, hit[i].transform.GetComponent<FactObject>().URI, true);
            }
        }
    }
  
}
