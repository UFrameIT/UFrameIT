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


public class BackToButton_mobile : MonoBehaviour
{
    public GameObject backTo_GObj;
    public GameObject parentM_GObj;


    //public GameObject back_GObj;

    void Start()
    {
        
    }

    private void Update()
    {

    }
    
    public void goBackButtonOPTM()
    {

        NetworkJSON_Save();

        ClearUIC();
        //UIconfig.Andr_Start_menue_counter = 1;
        backTo_GObj.SetActive(true); ;


        /*

        switch (CommunicationEvents.Andr_Start_menue_counter)
        {
            case 2:
                Manue0();
                break;

            case 3:
                OptionsMB();
                break;

            case 4:
                ChangeIPMButton();
                break;

            case 5:
                Inputfieldmenue();
                break;
            case 6:
                OptionsMB();
                break;

            default:
                Manue0();
                break;
        */
        
        
    

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
