using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MMTInterface : MonoBehaviour
{
    // Start is called before the first frame update

    
    void Start()
    {
        CommunicationEvents.AddEvent.AddListener(AddFactToMMT);
        CommunicationEvents.RemoveEvent.AddListener(RemoveFactFromMMT);     
    }

    void AddFactToMMT(RaycastHit hit, int id )
    {
        //send new fact to MMT
        Debug.Log("add fact"+ id);
    }

    void RemoveFactFromMMT(RaycastHit hit, int id)
    {
        Debug.Log("remove fact"+ id);
    }

 
}
