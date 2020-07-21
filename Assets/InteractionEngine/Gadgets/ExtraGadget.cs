using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class ExtraGadget : Gadget
{
    public WorldCursor Cursor;

    void Awake()
    {
        if (FactManager == null) FactManager = GameObject.FindObjectOfType<FactManager>();
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
        if (this.Cursor == null) this.Cursor = GameObject.FindObjectOfType<WorldCursor>();
    }

    void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
    }

    public override void OnHit(RaycastHit hit)
    {

    }

}
