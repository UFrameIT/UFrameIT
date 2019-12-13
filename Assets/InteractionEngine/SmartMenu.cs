using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMenu : MonoBehaviour
{

    public void DestroyObject()
    {
      
        CommunicationEvents.RemoveFactEvent.Invoke(CommunicationEvents.Facts[transform.parent.GetComponent<FactObject>().Id]);
       
    }


}
