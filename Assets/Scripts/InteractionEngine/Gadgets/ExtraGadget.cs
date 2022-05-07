using UnityEngine;

public class ExtraGadget : Gadget
{
    new void Awake()
    {
        base.Awake();
        UiName = "Extra Mode";
        MaxRange = GlobalBehaviour.GadgetPhysicalDistance;
    }

    new void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnHit(RaycastHit[] hit)
    {

    }

}
