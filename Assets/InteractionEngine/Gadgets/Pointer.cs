using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Pointer : Gadget
{
    public WorldCursor Cursor;

    void Awake()
    {
        if (FactManager == null) FactManager = GameObject.FindObjectOfType<FactManager>();
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
        if (this.Cursor == null) this.Cursor = GameObject.FindObjectOfType<WorldCursor>();
    }

    /*
    public override void activate() {
        this.activated = true;
        this.Cursor.setLayerMask(this.layerMask);
    }
    */

    void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
    }

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
