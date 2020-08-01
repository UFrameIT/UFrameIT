using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMenu : MonoBehaviour
{

    public FactManager FactManager;

    public void DestroyObject()
    {
      FactManager.DeleteFact(CommunicationEvents.Facts[transform.parent.GetComponent<FactObject>().Id]);
      //  CommunicationEvents.RemoveFactEvent.Invoke(CommunicationEvents.Facts[transform.parent.GetComponent<FactObject>().Id]);
       
    }


}
