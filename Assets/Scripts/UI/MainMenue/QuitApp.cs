using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;
//using static CheckServer;
///using static CommunicationEvents;
using static StreamingAssetLoader;
using static UIconfig;
using UnityEngine.EventSystems;


public class QuitApp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    void Start()
    {
        
    }

    private void Update()
    {

    }

    public void OnPointerDown(PointerEventData data)
    {

    }


    public void OnPointerUp(PointerEventData data)
    {
        QuitApp1();

    }

    public void QuitApp1()
    {

        //Input.backButtonLeavesApp = true;
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
        
    }
}







