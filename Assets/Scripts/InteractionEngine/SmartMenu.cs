using UnityEngine;

public class SmartMenu : MonoBehaviour
{

    public FactManager FactManager;

    public void DestroyObject()
    {
        CommunicationEvents.Facts.Remove(CommunicationEvents.Facts[transform.parent.GetComponent<FactObject>().Id]);
    }


}
