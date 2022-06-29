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

public class SaveNetwAddr_mobile : MonoBehaviour
{

    public GameObject SaveSlot1_B_GObj;
    public GameObject SaveSlot2_B_GObj;
    public GameObject SaveSlot3_B_GObj;

    private Color32 firstColB;
    private float transCol;
    private ColorBlock tempColB;

    public string Ip1_sup;
    public string Ip2_sup;
    public string Ip3_sup;


    void Start()
    {
        Ip1_sup = CommunicationEvents.IPslot1;
        Ip2_sup = CommunicationEvents.IPslot2;
        Ip3_sup = CommunicationEvents.IPslot3;


    }

    private void Update()
    {


        UpdateUI_3_f();
        UpdateUI_4_f();

        UpdateUI_5_f();
    }

    public void Refresh_pressed()
    {
        Ip1_sup = CommunicationEvents.IPslot1;
        Ip2_sup = CommunicationEvents.IPslot2; 
        Ip3_sup = CommunicationEvents.IPslot3;
        print("se"+Ip1_sup);
       
    }





    void UpdateUI_3_f()
    {
        //------------------------------------------------------------------------
        tempColB = SaveSlot1_B_GObj.GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        
            if (CommunicationEvents.ServerRunningA[3] == 0)
            {
                tempColB.normalColor = colOffline;
                if (string.IsNullOrEmpty(CommunicationEvents.IPslot1))// || CommunicationEvents.IPslot1.Length < 1)
                {
                    SaveSlot1_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
                }
                else
                {

                    
                    if (Ip1_sup.Equals(CommunicationEvents.IPslot1))
                    {
                        SaveSlot1_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot1 + " (Offline)";
                    }
                    else
                    {
                        print("sss" + Ip1_sup.Equals(CommunicationEvents.IPslot1)); 
                        SaveSlot1_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot1;
                    }
                }
            }
            else
            {
                if (CommunicationEvents.ServerRunningA[3] == 2)
                {
                    print("test" + Ip1_sup);
                    if (Ip1_sup.Equals(CommunicationEvents.IPslot1))
                    {
                        tempColB.normalColor = colOnline;
                        SaveSlot1_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot1 + " (Online)";
                    }
                    else
                    {
                    print("s3ss" + Ip1_sup.Equals(CommunicationEvents.IPslot1));
                    SaveSlot1_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot1;
                    print("ww"+ Ip1_sup);
                    print("we"+ CommunicationEvents.IPslot1);

                    }    
                }
                else
                {
                    tempColB.normalColor = colOffline;
                    SaveSlot1_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
                }
            }
            SaveSlot1_B_GObj.GetComponent<Button>().colors = tempColB;
        



    }




    void UpdateUI_4_f()
    {
        //-------------------------------------------------------------------------
        tempColB = SaveSlot2_B_GObj.GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[4] == 0)
        {
            tempColB.normalColor = colOffline;
            if (string.IsNullOrEmpty(CommunicationEvents.IPslot2))// || CommunicationEvents.IPslot2.Length<1)
            {
                SaveSlot2_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
            else
            {
                if (Ip2_sup.Equals(CommunicationEvents.IPslot2))
                {
                    SaveSlot2_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot2 + " (Offline)";
                }
                else
                {
                    SaveSlot2_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot2;
                }
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[4] == 2)
            {
                if (Ip2_sup.Equals(CommunicationEvents.IPslot2))
                {
                    tempColB.normalColor = colOnline;
                    SaveSlot2_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot2 + " (Online)";
                }
                else
                {
                    SaveSlot2_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot2;

                }
            }
            else
            {
                tempColB.normalColor = colOffline;
                SaveSlot2_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
        }
        SaveSlot2_B_GObj.GetComponent<Button>().colors = tempColB;
    }


    void UpdateUI_5_f()
    {
        //---------------------------------------------------------------------------
        tempColB = SaveSlot3_B_GObj.GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[5] == 0)
        {
            tempColB.normalColor = colOffline;
            if (string.IsNullOrEmpty(CommunicationEvents.IPslot3))// || CommunicationEvents.IPslot3.Length < 1)
            {
                SaveSlot3_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
            else
            {
                if (Ip3_sup.Equals(CommunicationEvents.IPslot3))
                {
                    SaveSlot3_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot3 + " (Offline)";
                }
                else
                {
                    SaveSlot3_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot3;
                }
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[5] == 2)
            {
                if (Ip3_sup.Equals(CommunicationEvents.IPslot3))
                {
                    tempColB.normalColor = colOnline;
                    SaveSlot3_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot3 + " (Online)";
                }
                else
                {
                    SaveSlot3_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot3;

                }
            }
            else
            {
                tempColB.normalColor = colOffline;
                SaveSlot3_B_GObj.GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
        }
        SaveSlot3_B_GObj.GetComponent<Button>().colors = tempColB;
        
    }


    public void Slot1_s()
    {
        CommunicationEvents.IPslot1 = CommunicationEvents.newIP;
        //CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress1;
        Update();
        CheckServerA[3] = 1;
        NetworkJSON_Save();
        Refresh_pressed();

        //CSform.CheckIPAdr();

    }

    public void Slot2_s()
    {

        //CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress2;
        CommunicationEvents.IPslot2 = CommunicationEvents.newIP;
        Update();
        
        CheckServerA[4] = 1;

        NetworkJSON_Save();
        Refresh_pressed();
        //CSform.CheckIPAdr();


    }

    public void Slot3_s()
    {
        //CheckServer ani = new CheckServer();//= obj.AddComponent<CheckServer>();
        // ani.StartCheck();
        CommunicationEvents.IPslot3 = CommunicationEvents.newIP;
        Update();
        CheckServerA[5] = 1;

        Refresh_pressed();

        NetworkJSON_Save();
        //CSform.CheckIPAdr();
    }
}

















