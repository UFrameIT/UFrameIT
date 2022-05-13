using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using System.IO; 
using UnityEngine.Video;//streaming
using UnityEngine.Networking;
//using static StreamingAssetLoader;
//using static CheckServer;
//using static CommunicationEvents;
using static UIconfig;


public class MainPlayerMode_Ctrl : MonoBehaviour
{


    //public int myUI_ID;
    public GameObject myself_GObj;
    //public GameObject parent_GObj;
    //public int backUI_ID;
    //public int optionsUI_ID;
    //public int failedUI_ID;
    public GameObject FirstPerson_GObj;
    public GameObject FirstPersonOldInpOrig_GObj;
    public GameObject ThirdPerson_GObj;
    
    private int GpMode_before=-99;

    void Start()
    {
        
        Update2(); 
        
        
    }

    private void Update()
    {
        if (GpMode_before!= UIconfig.GameplayMode)
        {
            Update2();
            print(UIconfig.GameplayMode);
        }

    }

    private void Update2(){

        
        //Todo Eventbased
        ClearUIC();
        print("hey"+ UIconfig.GameplayMode);
        switch (UIconfig.GameplayMode)
        {
            case 0:
                
                break;
            case 1:
                
                break;
            case 2:
                
                break;
            case 3:
                
                break;
            case 4:
                ThirdPerson_GObj.gameObject.SetActive(true);
                break;
            case 5:
                FirstPerson_GObj.gameObject.SetActive(true);
                break;
            case 6:
                FirstPersonOldInpOrig_GObj.gameObject.SetActive(true);
                break;
            default:
                
                break;
        }
        GpMode_before = UIconfig.GameplayMode;

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