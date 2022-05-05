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


public class GameObj_OnOff : MonoBehaviour
{

    public GameObject Target_GObj;
    public int myUI_ID;
    public int default_value;



    void Start()
    {
        if (CheckArray())
        {
            if (UIconfig.CanvasOnOff_Array[myUI_ID]!=2 || UIconfig.CanvasOnOff_Array[myUI_ID] != 3) {
                UIconfig.CanvasOnOff_Array[myUI_ID] = default_value;
                //Update();
            }
        }
    }

    private void Update()
    {
        if (CheckArray())
        {
            switch (UIconfig.CanvasOnOff_Array[myUI_ID])
            {
                case 1:
                    ActivateUIC_target();
                    break;
                case 0:
                    ClearUIC_target();
                    break;
                case 3:
                    ActivateUIC_target();
                    break;
                case 2:
                    ClearUIC_target();
                    break;
                default:
                    ClearUIC_target();
                    break;
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


    /// <summary>
    /// Deactivates all Pages.
    /// </summary>
    private void ActivateUIC_target()
    {

        
            Target_GObj.gameObject.SetActive(true);
        
    }


    /// <summary>
    /// Deactivates all Pages.
    /// </summary>
    private void ClearUIC_target()
    {
        
        
            Target_GObj.gameObject.SetActive(false);
        
    }

    
}