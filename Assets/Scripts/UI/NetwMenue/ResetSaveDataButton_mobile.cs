using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;

using static UIconfig;
using static StreamingAssetLoader;


public class ResetSaveDataButton_mobile : MonoBehaviour
{




    void Start()
    {

    }

    private void Update()
    {

    }

    public void resetPlayerConfig_Bttn()
    {
        ResetPlayerConfig();
    }

    public void resetPlayerSaveGame_Bttn()
    {
        //ResetPlayerSaveGame();
        ResetDataPath();
        print("resetPlayerSaveGame_Bttn, Todo");
    }

    public void resetDataPath_Bttn()
    {
        //ResetPlayerSaveGame();
        ResetDataPath();
    }

    public void ResetAllSaveData_Bttn()
    {
        ResetStreamingAsset();
    }
}