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
using static CheckServer;
using static LaunchMenue_mobile;

public class SelectedNewNetwServer_mobile : MonoBehaviour
{
    public GameObject NewNAddrW;
    public GameObject NewNAddrT;
    public GameObject NewNAddrTS;
    public GameObject NewNAddrTI;
    public GameObject StartGwoS;
    

    private Color32 firstColB;
    private float transCol;
    private ColorBlock tempColB;


    void Start()
    {
        Update();
    }



    private void Update()
    {

        UpdateUI_6_f();
      
    }

    void UpdateUI_6_f()
    {
        //--------------------------------------------------------------------------

        if (CommunicationEvents.ServerRunningA[6] == 0 || CommunicationEvents.ServerRunningA[6] == 3)
        {
            NewNAddrW.GetComponent<Image>().color = colOffline;
            StartGwoS.GetComponent<Image>().color = colClear;
            StartGwoS.GetComponentInChildren<Text>().color = colClear;
            //GameObject.Find("StartGwoS").GetComponent<Image>().color = colClear;
            //GameObject.Find("SGwoST").GetComponent<Text>().color = colClear;
            if (string.IsNullOrEmpty(CommunicationEvents.selecIP))//; || ())
            {
                NewNAddrTI.GetComponent<Text>().text = "< empty >";
                NewNAddrTS.GetComponent<Text>().text = "Status: no ip-address";
            }
            else
            {
                NewNAddrTI.GetComponent<Text>().text = CommunicationEvents.selecIP;


                if (CommunicationEvents.ServerRunningA[6] == 0)
                {
                    NewNAddrTS.GetComponent<Text>().text = "Status: Offline";


                }
                else
                {
                    NewNAddrTS.GetComponent<Text>().text = "Status: no ip-address";
 
                }

            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[6] == 2)
            {
                NewNAddrW.GetComponent<Image>().color = colOnline;
                NewNAddrTI.GetComponent<Text>().text = CommunicationEvents.selecIP;
                NewNAddrTS.GetComponent<Text>().text = "Status: Online";
                StartGwoS.GetComponent<Image>().color = Color.white;
                StartGwoS.GetComponentInChildren<Text>().color = Color.black;
    
            }
            else
            {
                StartGwoS.GetComponent<Image>().color = colClear;
                StartGwoS.GetComponentInChildren<Text>().color = colClear;
                NewNAddrW.GetComponent<Image>().color = colOffline;

                if (string.IsNullOrEmpty(CommunicationEvents.selecIP))//; || ())
                {
                    NewNAddrTI.GetComponent<Text>().text = "< empty >";

                    NewNAddrTS.GetComponent<Text>().text = "Status: no ip-address";

                }
                else
                {


                    NewNAddrTI.GetComponent<Text>().text = CommunicationEvents.selecIP;
                    NewNAddrTS.GetComponent<Text>().text = "Status: unknown";//"Status: checking";
                }


            }
        }
    }

    public void startNewIPGameButton()
    {
        if ((CommunicationEvents.ServerRunningA[6] == 2) && (CommunicationEvents.ServerRunningA[2] == 2))
        {
            startNextSceneFunctionNewGame();
        }
        else
        {
            
        }
    }



}
















