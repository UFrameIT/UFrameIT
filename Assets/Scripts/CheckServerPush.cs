using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static CommunicationEvents;

public class CheckServerPush : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public int CheckServer_0;
    public int CheckServer_1;
    public int CheckServer_2;
    public int CheckServer_3;
    public int CheckServer_4;
    public int CheckServer_5;
    public int CheckServer_6;

    void Start()
    {
       
    }

    private void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CheckServerA[0] = CheckServer_0;
        CheckServerA[1] = CheckServer_1;
        CheckServerA[2] = CheckServer_2;
        CheckServerA[3] = CheckServer_3;
        CheckServerA[4] = CheckServer_4;
        CheckServerA[5] = CheckServer_5;
        CheckServerA[6] = CheckServer_6;

    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
}


