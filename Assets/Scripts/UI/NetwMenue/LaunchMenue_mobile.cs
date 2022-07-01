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
//new InputSystem
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;


public class LaunchMenue_mobile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{



    public GameObject parentM_GObj;
    public GameObject optionsNetwM_GObj;

    public void OnPointerDown(PointerEventData eventData)
    {
        StartWithLastServerButton();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        
    }




    void Start()
    {
        

    }

    private void Update()
    {
        //UnityEngine.Debug.Log("StartMainMenue");
    }


    public void StartWithLastServerButton()
    {

        if (CommunicationEvents.ServerRunningA[6] == 2)
        {
            startNextSceneFunctionNewGame();
        }
        else
        {
            toNetwOptionsM();
        }

    }



    public void toNetwOptionsM()
    {
        ClearUIC();
        UIconfig.Andr_Start_menue_counter = 2;
        optionsNetwM_GObj.SetActive(true);



    }

    /// <summary>
    /// Deactivates all Pages.
    /// </summary>
    private  void ClearUIC()
    {

        for (int i = 0; i < parentM_GObj.transform.childCount; i++)
        {
            parentM_GObj.transform.GetChild(i).gameObject.SetActive(false);
        }
    }



    
    public static void startNextSceneFunctionNewGame()
    {
        NetworkJSON_Save();
        CommunicationEvents.ServerAdress = "http://" + CommunicationEvents.selecIP;
        CommunicationEvents.ServerRunning = true;
        UnityEngine.Debug.Log("StartMainMenue");
        UnityEngine.Debug.Log("CommunicationEvents.ServerAdress = " + CommunicationEvents.ServerAdress);
        SceneManager.LoadScene("MainMenue");
    }

}


  



