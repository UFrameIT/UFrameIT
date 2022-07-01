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


public class MenueControl_mobile : MonoBehaviour
{


    //public int myUI_ID;
    public GameObject myself_GObj;
    //public GameObject parent_GObj;
    //public int backUI_ID;
    //public int optionsUI_ID;
    //public int failedUI_ID;
    public GameObject child1_GObj;

    void Start()
    {
        toChild1();
    }

    private void Update()
    {
        
    }



    public void toChild1()
    {
        ClearUIC();
        //UIconfig.Andr_Start_menue_counter = 1;
        child1_GObj.SetActive(true); 


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