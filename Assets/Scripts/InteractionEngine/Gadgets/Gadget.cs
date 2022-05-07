using UnityEngine;

/// <summary>
/// Base class for all Gadgets to derive from.
/// A Gadget is a tool for the player (and level editor) to interact with the GameWorld.
/// </summary>
public class Gadget : MonoBehaviour
{
    /// <summary> Position in tool belt. </summary>
    /// <remarks>Set in Inspector or <see cref="Awake"/></remarks>
    public int id = -1;
    /// <summary> Tool Name </summary>
    /// <remarks>Set in Inspector or <see cref="Awake"/></remarks>
    public string UiName = "Name Not Set";
    /// <summary> Maximum range for this Tool. For consistency use GadgetDistances in <see cref="GlobalBehaviour"/>.</summary>
    /// <remarks>Set in Inspector or <see cref="Awake"/></remarks>
    public float MaxRange;

    /// <summary>Which sprite to use</summary>
    /// <remarks>Set in Inspector</remarks>
    public Sprite Sprite;
    /// <summary>Layers to ignore for thid gadget by default.</summary>
    /// <remarks>Set in Inspector</remarks>
    public LayerMask ignoreLayerMask;
    /// <summary>Which cursor to use</summary>
    /// <remarks>When not set in Inspector, will be searching for any <see cref="WorldCursor"/>.</remarks>
    public WorldCursor Cursor;

    protected void Awake()
    {
        if (Cursor == null)
            Cursor = GameObject.FindObjectOfType<WorldCursor>();

        CommunicationEvents.TriggerEvent.AddListener(OnHit);
    }

    protected void OnEnable()
    {
        this.Cursor.setLayerMask(~this.ignoreLayerMask.value);
        Cursor.MaxRange = MaxRange;
    }

    /// <summary>
    /// Called when <see cref="CommunicationEvents.TriggerEvent"/> is invoked, a.k.a. when Player clicks in GameWorld.
    /// </summary>
    /// <param name="hit">the position where it was clicked</param>
    public virtual void OnHit(RaycastHit[] hit) { }
}
