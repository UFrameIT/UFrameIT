using UnityEngine;


public class Gadget : MonoBehaviour
{

    public int id = -1;
    public string UiName = "Name Not Set";
    public Sprite Sprite;
    public FactManager FactManager;
    public LayerMask ignoreLayerMask;
    public WorldCursor Cursor;
    public float MaxRange;

    protected void Awake()
    {
        if (FactManager == null)
            FactManager = GameObject.FindObjectOfType<FactManager>();

        if (Cursor == null)
            Cursor = GameObject.FindObjectOfType<WorldCursor>();

        CommunicationEvents.TriggerEvent.AddListener(OnHit);
    }

    protected void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
        Cursor.MaxRange = MaxRange;
    }

    public virtual void OnHit(RaycastHit hit) { }
}
