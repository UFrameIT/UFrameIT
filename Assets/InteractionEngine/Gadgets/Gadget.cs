using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gadget : MonoBehaviour
{

    public ToolMode ToolMode;
    public Sprite Sprite;
    public FactManager FactManager;
    // Start is called before the first frame update
    void Start()
    {
        if (FactManager == null) FactManager = GameObject.FindObjectOfType<FactManager>();
        CommunicationEvents.TriggerEvent.AddListener(OnHit);

    }

    public virtual void OnHit(RaycastHit hit)
    {

    }

}
