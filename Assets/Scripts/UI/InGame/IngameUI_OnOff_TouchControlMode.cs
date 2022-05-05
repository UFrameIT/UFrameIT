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


public class IngameUI_OnOff_TouchControlMode : MonoBehaviour
{

    public GameObject myself_GObj;
    public int myControlMode_ID;
    public int myTouchControlMode_ID;
    




    void Start()
    {
        
            
            Update();
        
    }

    private void Update()
    {
        if (CheckArray())
        {
            if (UIconfig.touchControlMode == myTouchControlMode_ID && UIconfig.controlMode == myControlMode_ID)
            {
                ActivateUIC();
            }
            else
            {
                ClearUIC();
            }
        }
    }
    
    private bool CheckArray()
    {
        /*
        if (myTouchControlMode_ID != null && myControlMode_ID != null)
        {
            return true;

        }
        return false;
        */
        return true;
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