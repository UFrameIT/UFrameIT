using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;
using static CommunicationEvents;
using static UIconfig;
using static StreamingAssetLoader;


public class BackToButton_withConfChange : MonoBehaviour
{
    public GameObject backTo_GObj;
    public GameObject parentM_GObj;
    public int GameplayMode; 


    //public GameObject back_GObj;

    void Start()
    {
        
    }

    private void Update()
    {

    }
    
    public void goBackButtonOPTM()
    {

        //NetworkJSON_Save();
        
        ClearUIC();
 
        backTo_GObj.SetActive(true); ;

        UIconfig.GameplayMode = GameplayMode;

        
        
    

    }




   

 

    /// <summary>
    /// Deactivates all Pages.
    /// </summary>
    private void ClearUIC()
    {
        if (parentM_GObj == null) { return; }

        for (int i = 0; i < parentM_GObj.transform.childCount; i++)
        {
            parentM_GObj.transform.GetChild(i).gameObject.SetActive(false);
        }
    }






}
