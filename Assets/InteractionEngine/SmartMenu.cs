using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartMenu : MonoBehaviour
{

    public void DestroyObject()
    {
        CommunicationEvents.RemoveEvent.Invoke(transform.parent.GetComponent<FactObject>().Id);
       
    }


}
