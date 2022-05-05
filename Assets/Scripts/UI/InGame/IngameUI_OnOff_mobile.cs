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


public class IngameUI_OnOff_mobile : MonoBehaviour
{

    public GameObject myself_GObj;
    public int myUI_ID;
    public int default_value;
    private bool cA;


    void Start()
    {
        cA = CheckArray();
        if (cA)
        {
            if (UIconfig.CanvasOnOff_Array[myUI_ID] != 2 || UIconfig.CanvasOnOff_Array[myUI_ID] != 3)
            {
                
                UIconfig.CanvasOnOff_Array[myUI_ID] = default_value;
                //Update();
            }
        }
    }

    private void Update()
    {
        if (cA)
        {
            switch (UIconfig.CanvasOnOff_Array[myUI_ID])
            {
                case 1:
                    ActivateUIC();
                    break;
                case 0:
                    ClearUIC();
                    break;
                case 3:
                    ActivateUIC();
                    break;
                case 2:
                    ClearUIC();
                    break;

                default:
                    ClearUIC();
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
    private void ActivateUIC()
    {

        for (int i = 0; i < myself_GObj.transform.childCount; i++)
        {
            myself_GObj.transform.GetChild(i).gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// Deactivates all Pages.
    /// </summary>
    private void ClearUIC()
    {
        
        for (int i = 0; i < myself_GObj.transform.childCount; i++)
        {
            myself_GObj.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    
}