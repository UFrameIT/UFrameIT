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
using static Restart;
using static SceneSwitcher;



/*
  https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/#:~:text=The%20most%20convenient%20method%20for%20pausing%20the%20game,will%20return%20the%20game%20to%20its%20normal%20speed.
*/

public class TimeStop: MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

   

    public bool checkTimeToStop;





    void Start()
    {
        PauseGame();
    }

    private void Update()
    {
        if (checkTimeToStop == true) {
            PauseGame();
            //disableGameUI();
        }
    }

    public void OnPointerDown(PointerEventData data)
    {

    }

    public void OnPointerUp(PointerEventData data)
    {

       
    }

       

    private void PauseGame()
    {

        UIconfig.GamePaused = true; 
        Time.timeScale = 0;
    }







}