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


public class InputUI_Ctrl_mobile : MonoBehaviour
{


    //public int myUI_ID;
    public GameObject myself_GObj;
    //public GameObject parent_GObj;
    //public int backUI_ID;
    //public int optionsUI_ID;
    //public int failedUI_ID;
    public GameObject FrameITUI_GObj;
    public GameObject FrameITUI_mobile_GObj;
    private int FrameITUIversion_temp =-1;

    void Start()
    {
        Update();
    }

    private void Update()
    {
        switch (UIconfig.FrameITUIversion)
        {
            case 1:
                toMouseUI();
                break;
            case 2:
                toMobileUI();
                break;
        }
    }

    public void toMouseUI()
    {
        if (FrameITUIversion_temp != 1)
        {
            toMouseUI2();
            FrameITUIversion_temp = 1;
        }
    }
    public void toMobileUI()
    {
        if (FrameITUIversion_temp != 2)
        {
            toMobileUI2();
            FrameITUIversion_temp = 2;
        }
    }

    public void toMouseUI2()
    {
        ClearUIC();
        //UIconfig.Andr_Start_menue_counter = 1;
        FrameITUI_GObj.SetActive(true); ;


    }

    public void toMobileUI2()
    {
        ClearUIC();
        //UIconfig.Andr_Start_menue_counter = 1;
        FrameITUI_mobile_GObj.SetActive(true); ;


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