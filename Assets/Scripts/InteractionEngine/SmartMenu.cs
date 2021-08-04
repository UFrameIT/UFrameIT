using UnityEngine;

public class SmartMenu : MonoBehaviour
{

    public FactManager FactManager;

    public void DestroyObject()
    {
        CommunicationEvents.LevelFacts.Remove(CommunicationEvents.LevelFacts[transform.parent.GetComponent<FactObject>().URI]);
    }


}
