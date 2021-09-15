using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Remover : Gadget
{
    new void Awake()
    {
        base.Awake();
        this.UiName = "Remove Mode";
        if (MaxRange == 0)
            MaxRange = GlobalBehaviour.GadgetLaserDistance;
    }

    public override void OnHit(RaycastHit hit)
    {

        if (!this.isActiveAndEnabled)
            return;

        // TODO: ask/warn user to cascade
        var hid = hit.transform.GetComponent<FactObject>().URI;
        StageStatic.stage.factState.Remove(hid);
    }
}
