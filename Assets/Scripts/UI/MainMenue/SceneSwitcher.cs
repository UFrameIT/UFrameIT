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


public class SceneSwitcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int switchToScene_ID;
    
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
        NowsSwitchToScene(switchToScene_ID);

    }

    public void NowsSwitchToScene(int switchToSceneID_2)
    {


        switch(switchToSceneID_2)
        {
            case 3:  
                    SceneManager.LoadScene("LaunchMenue");
                    break;
            case 4:  
                    SceneManager.LoadScene("MainMenue");
                    break;
            default:
                    SceneManager.LoadScene("LaunchMenue");
                    break;

        }        
        
        
    }
}







