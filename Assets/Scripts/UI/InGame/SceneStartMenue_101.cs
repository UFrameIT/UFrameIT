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
using static StageStatic;



/*
  https://gamedevbeginner.com/the-right-way-to-pause-the-game-in-unity/#:~:text=The%20most%20convenient%20method%20for%20pausing%20the%20game,will%20return%20the%20game%20to%20its%20normal%20speed.
*/

public class SceneStartMenue_101 : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler
{

    //public GameObject myself_GObj_Txt;
    public Text myself_GObj_Txt;
    public int myself_Txt_ID;
    //public int myUI_ID;
    //public int setValueTo;
    //public bool ResetLevel;
    //public bool checkTimeToStop;
    //public bool ResetUI;
    //public int switchToScene_ID_;
    string missionbriefing = "";
    string missionbriefing1 = "After a long trip through the forest, you have found a resting place near a stream. After drinking some water you have heared cracking of branches. It seems you are not alone.";
    string missionbriefing330 = "as a Sidescroller Game with camera change";
    string missionbriefing331 = "as a Sidescroller Game";
    string missionbriefing332 = "as a Escaperoom Game";
    string missionbriefing333 = "as a Third Person Game with high Camera";
    string missionbriefing334 = "as a Third Person Game";
    string missionbriefing335 = "as a First Person Game";
    string missionbriefing336 = "as a First Person Game with old mainplayer version";



    void Start()
    {
        

        //myself_GObj_Txt.text = "hello World";
        //StageStatic StgStc = new StageStatic();
        //myself_GObj_Txt.text = "ssss"+StageStatic.stage.number;
        //myself_GObj_Txt.text = "ssss" + StageStatic.stage.scene;
        //myself_GObj_Txt.text = "ssss" + StageStatic.stage.description;

        switch (myself_Txt_ID) {
            case 1 :
                myself_GObj_Txt.text = "World: " + StageStatic.stage.scene + "\n" +
                                        "Stage: " + StageStatic.stage.name + "\n"; 
                                                //"Info:" + StageStatic.stage.description;
                        break;
            case 2:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing;
                break;
            case 330:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing330;
             
                break;
            case 331:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing331;
               
                break;
            case 332:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing332;
               
                break;
            case 333:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing333;
                  
                break;
            case 334:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing334;
                   
                break;
            case 335:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing335;
                  
                break;
            case 336:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing336;

                break;
            case 337:
                missionbriefing = missionbriefing1;
                myself_GObj_Txt.text = StageStatic.stage.description + "\n" +
                    //"\n"+
                    missionbriefing336;

                break;

        }

    }

    private void Update()
    {

    }
}

