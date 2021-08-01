using UnityEngine;

public class ExtraGadget : Gadget
{
    public WorldCursor Cursor;

    void Awake()
    {
        if (FactManager == null)
            FactManager = GameObject.FindObjectOfType<FactManager>();

        if (this.Cursor == null)
            this.Cursor = GameObject.FindObjectOfType<WorldCursor>();

        this.UiName = "Extra Mode";
        CommunicationEvents.TriggerEvent.AddListener(OnHit);
    }

    void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
    }

    public override void OnHit(RaycastHit hit)
    {

    }

}
