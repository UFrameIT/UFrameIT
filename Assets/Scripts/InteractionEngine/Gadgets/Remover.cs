using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Remover : Gadget
{
    public WorldCursor Cursor;

    void Awake()
    {
        if (FactManager == null)
            FactManager = GameObject.FindObjectOfType<FactManager>();

        if (this.Cursor == null)
            this.Cursor = GameObject.FindObjectOfType<WorldCursor>();

        this.UiName = "Remove Mode";
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
    }

    void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
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
