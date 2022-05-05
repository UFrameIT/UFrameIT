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

public class SelectedNetwServer_mobile : MonoBehaviour
{
    public GameObject SelNAddrW;
    public GameObject SelNAddrT;
    public GameObject SelNAddrTS;
    public GameObject SelNAddrTI;
    public GameObject SSstGame;


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
            SelNAddrW.GetComponent<Image>().color = colOffline;
            SSstGame.GetComponent<Image>().color = colClear;
            SSstGame.GetComponentInChildren<Text>().color = colClear;
            //GameObject.Find("StartGwoS").GetComponent<Image>().color = colClear;
            //GameObject.Find("SGwoST").GetComponent<Text>().color = colClear;
            if (string.IsNullOrEmpty(CommunicationEvents.selecIP))//; || ())
            {
                SelNAddrTI.GetComponent<Text>().text = "< empty >";
                //GameObject.Find("newNAddrTI").GetComponent<Text>().text = "< empty >";
                SelNAddrTS.GetComponent<Text>().text = "Status: no server";
                //GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: no server";
                //GameObject.Find("SelNAddrTS").GetComponent<Text>().text = "Status: no ip-address";
                //GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: no ip-address";
            }
            else
            {
                SelNAddrTI.GetComponent<Text>().text = CommunicationEvents.selecIP;
                //GameObject.Find("newNAddrTI").GetComponent<Text>().text = CommunicationEvents.selecIP;

                if (CommunicationEvents.ServerRunningA[6] == 0)
                {
                    SelNAddrTS.GetComponent<Text>().text = "Status: Offline";
                    //GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: Offline";

                }
                else
                {
                    SelNAddrTS.GetComponent<Text>().text = "Status: no ip-address";
                    //GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: no ip-address";
                }

            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[6] == 2)
            {
                SelNAddrW.GetComponent<Image>().color = colOnline;
                SelNAddrTI.GetComponent<Text>().text = CommunicationEvents.selecIP;
                SelNAddrTS.GetComponent<Text>().text = "Status: Online";
                SSstGame.GetComponent<Image>().color = Color.white;
                SSstGame.GetComponentInChildren<Text>().color = Color.black;

            }
            else
            {

                SSstGame.GetComponent<Image>().color = colClear;
                SSstGame.GetComponentInChildren<Text>().color = colClear;
                SelNAddrW.GetComponent<Image>().color = colOffline;

                if (string.IsNullOrEmpty(CommunicationEvents.selecIP))//; || ())
                {
                    SelNAddrTI.GetComponent<Text>().text = "< empty >";

                    SelNAddrTS.GetComponent<Text>().text = "Status: no server";

                }
                else
                {



                    SelNAddrTI.GetComponent<Text>().text = CommunicationEvents.selecIP;
                    SelNAddrTS.GetComponent<Text>().text = "Status: checking";

                }

            }
        }
    }


    public void StartWithSelectedServerButton()
    {

        if (CommunicationEvents.ServerRunningA[6] == 2)
        {
            startNextSceneFunctionNewGame();
        }
        else
        {

        }

    }





}
















