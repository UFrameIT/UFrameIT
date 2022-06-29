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
using static Restart_script;
//using static SceneSwitcher;



/*
  https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/#:~:text=The%20most%20convenient%20method%20for%20pausing%20the%20game,will%20return%20the%20game%20to%20its%20normal%20speed.
*/

public class Pause_Menue_mobile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    //public GameObject myself_GObj;
    public int myUI_ID;
    public int setValueTo;
    public bool ResetLevel;
    public bool checkTimeToStop;
    public bool ResetUI;
    public int switchToScene_ID_;




    void Start()
    {

    }

    private void Update()
    {
        if (checkTimeToStop == true) { 
            setPauseToken();
            //disableGameUI();
        }
    }

    public void OnPointerDown(PointerEventData data)
    {

    }

    public void OnPointerUp(PointerEventData data)
    {

        if (ResetUI)
        {
            ContinueGame_BttnPressed();
        }
        if (CheckArray())
        {
            UIconfig.CanvasOnOff_Array[myUI_ID] = setValueTo;
        }
        UIconfig.GamePaused = false;
        ResumeGame();

        

    }

    
    private void ContinueGame_BttnPressed()
    {
        for (int i = 0; i<UIconfig.CanvasOn_afterPM.Length; i++)
        {
            int a = UIconfig.CanvasOn_afterPM[i];
            if ( a>=0 && a< UIconfig.CanvasOnOff_Array.Length)
            {
                UIconfig.CanvasOnOff_Array[a] = 1;
            }
            
              
        }


    }

    private bool CheckArray()
    {
        if (myUI_ID >= 0 && myUI_ID < UIconfig.CanvasOnOff_Array.Length)
        {
            return true;
        }
        return false;
    }

    private void setPauseToken()
    {
        if (CheckArray())
        {
            if (UIconfig.CanvasOnOff_Array[myUI_ID] == 1)
            {
                
                if (CommunicationEvents.CursorVisDefault)
                {
                    Cursor.visible = true;
                }
                else
                {
                    Cursor.visible = false;
                }
                UIconfig.GamePaused = true;
                PauseGame();


            }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        //Time.timeScale = UIconfig.Game_TimeScale;
    }
    private void ResumeGame()
    {
        Time.timeScale = UIconfig.Game_TimeScale;
        if (ResetLevel)
        {
            //Restart resClass = new Restart();
            Restart_script resClass = new Restart_script();
            resClass.LevelReset();
        }
        if (switchToScene_ID_ > 0)
        {
            
            //SceneSwitcher ScSw = new SceneSwitcher();
            //ScSw.NowsSwitchToScene(switchToScene_ID_);
            //SceneManager.LoadScene("MainMenue");

            switch (switchToScene_ID_)
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







}