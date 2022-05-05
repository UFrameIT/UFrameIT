using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;
//using static StreamingAssetLoader;
//using static CheckServer;
//using static CommunicationEvents;
using static UIconfig;
using UnityEngine.EventSystems;


public class IngameUI_OnOffButton_mobile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    //public GameObject myself_GObj;
    public int myUI_ID;
    public int setValueTo;
    //public bool simulateKey;



    void Start()
    {

    }

    private void Update()
    {

    }

    private bool CheckArray()
    {
        if (myUI_ID >= 0 && myUI_ID < UIconfig.CanvasOnOff_Array.Length)
        {
            return true;
        }
        return false;
    }


    public void OnPointerDown(PointerEventData data)
    {

    }


    public void OnPointerUp(PointerEventData data)
    {
        if (CheckArray())
        {
            UIconfig.CanvasOnOff_Array[myUI_ID] = setValueTo;
        }

    }
}