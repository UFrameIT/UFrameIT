using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommunicationEvents;

public class Pointer : Gadget
{
    public WorldCursor Cursor;

    void Awake()
    {
        if (FactManager == null)
            FactManager = GameObject.FindObjectOfType<FactManager>();

        if (this.Cursor == null)
            this.Cursor = GameObject.FindObjectOfType<WorldCursor>();

        this.UiName = "Point Mode";
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
    }

    void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
    }

    public override void OnHit(RaycastHit hit)
    {

        if (!this.isActiveAndEnabled) return;
        var pid = FactManager.AddPointFact(hit).URI;

        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ray"))
        {
            FactManager.AddOnLineFact(pid, hit.transform.GetComponent<FactObject>().URI, true);
        }
    }
  
}
