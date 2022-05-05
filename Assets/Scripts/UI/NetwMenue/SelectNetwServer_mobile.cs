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

public class SelectNetwServer_mobile : MonoBehaviour
{
    public GameObject LPS_B_GObj;
    public GameObject Slot1_B_GObj;
    public GameObject Slot2_B_GObj;
    public GameObject Slot3_B_GObj;

    private Color32 firstColB;
    private float transCol;
    private ColorBlock tempColB;


    void Start()
    {
        Update();
    }

    private void Update()
    {
                 
            UpdateUI_1_f(); 
            UpdateUI_3_f();
            UpdateUI_4_f();
        
            UpdateUI_5_f();
    }

    public void SlotLPS()
    {

        //CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress1;


        CommunicationEvents.ServerRunningA[6] = CommunicationEvents.ServerRunningA[1];
        CommunicationEvents.selecIP = CommunicationEvents.lastIP;
        CommunicationEvents.IPcheckGeneration++;
        NetworkJSON_Save();

        //SceneManager.LoadScene("Andr_TreeWorld");       
        //SceneManager.LoadScene("MainMenue");


    }

    public void Slot1()
    {

        //CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress1;

        CommunicationEvents.ServerRunningA[6] = CommunicationEvents.ServerRunningA[3];
        CommunicationEvents.selecIP = CommunicationEvents.IPslot1;
        CommunicationEvents.IPcheckGeneration++;
        NetworkJSON_Save();

        //SceneManager.LoadScene("Andr_TreeWorld");       
        //SceneManager.LoadScene("MainMenue");


    }

    public void Slot2()
    {

        //CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress2;

        CommunicationEvents.ServerRunningA[6] = CommunicationEvents.ServerRunningA[4];
        CommunicationEvents.selecIP = CommunicationEvents.IPslot2;
        CommunicationEvents.IPcheckGeneration++;

        NetworkJSON_Save();

        //SceneManager.LoadScene("Andr_TreeWorld");       
        //SceneManager.LoadScene("MainMenue");
    }

    public void Slot3()
    {
        //CheckServer ani = new CheckServer();//= obj.AddComponent<CheckServer>();
        // ani.StartCheck();

        CommunicationEvents.ServerRunningA[6] = CommunicationEvents.ServerRunningA[5];
        CommunicationEvents.selecIP = CommunicationEvents.IPslot3;
        CommunicationEvents.IPcheckGeneration++;
        NetworkJSON_Save();


    }

    void UpdateUI_1_f()
    {
        tempColB = LPS_B_GObj.GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[1] == 0)
        {

            tempColB.normalColor = colOffline;

            LPS_B_GObj.GetComponent<Button>().colors = tempColB; // new Color(148, 229, 156);
            if (string.IsNullOrEmpty(CommunicationEvents.lastIP))// || CommunicationEvents.lastIP.Length < 4)
            {
                LPS_B_GObj.GetComponentInChildren<Text>().text = "No game played before";
            }
            else
            {
                LPS_B_GObj.GetComponentInChildren<Text>().text = "Last played on " + CommunicationEvents.lastIP + " (Offline)";
            }
        }
        else
        {

            if (CommunicationEvents.ServerRunningA[1] == 2)
            {
                tempColB.normalColor = colOnline;
                LPS_B_GObj.GetComponentInChildren<Text>().text = "Last played on " + CommunicationEvents.lastIP + " (Online)";
            }
            else
            {
                tempColB.normalColor = colOffline;
                LPS_B_GObj.GetComponentInChildren<Text>().text = "No game played before";
            }
        }
        LPS_B_GObj.GetComponent<Button>().colors = tempColB;
    }


    void UpdateUI_3_f()
    {
        //------------------------------------------------------------------------
        tempColB = Slot1_B_GObj.GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[3] == 0)
        {
            tempColB.normalColor = colOffline;
            if (string.IsNullOrEmpty(CommunicationEvents.IPslot1))// || CommunicationEvents.IPslot1.Length < 1)
            {
                Slot1_B_GObj.GetComponentInChildren<Text>().text = "< empty >";
                //GameObject.Find("IPSlot1_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
            else
            {

                Slot1_B_GObj.GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot1 + " (Offline)";
                //GameObject.Find("IPSlot1_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot1 + " (Offline)";
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[3] == 2)
            {
                tempColB.normalColor = colOnline;
                Slot1_B_GObj.GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot1 + " (Online)";
                //GameObject.Find("IPSlot1_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot1 + " (Online)";
            }
            else
            {
                tempColB.normalColor = colOffline;
                Slot1_B_GObj.GetComponentInChildren<Text>().text = "< empty >";
                //GameObject.Find("IPSlot1_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
        }
        Slot1_B_GObj.GetComponent<Button>().colors = tempColB;


    }




    void UpdateUI_4_f()
    {
        //-------------------------------------------------------------------------
        tempColB = Slot2_B_GObj.GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[4] == 0)
        {
            tempColB.normalColor = colOffline;
            if (string.IsNullOrEmpty(CommunicationEvents.IPslot2))// || CommunicationEvents.IPslot2.Length<1)
            {
                Slot2_B_GObj.GetComponentInChildren<Text>().text = "< empty >";
                //GameObject.Find("IPSlot2_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
            else
            {
                Slot2_B_GObj.GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot2 + " (Offline)";
                //GameObject.Find("IPSlot2_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot2 + " (Offline)";
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[4] == 2)
            {
                tempColB.normalColor = colOnline;
                Slot2_B_GObj.GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot2 + " (Online)";
                //GameObject.Find("IPSlot2_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot2 + " (Online)";
            }
            else
            {
                tempColB.normalColor = colOffline;
                Slot2_B_GObj.GetComponentInChildren<Text>().text = "< empty >";
                //GameObject.Find("IPSlot2_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
        }
        Slot2_B_GObj.GetComponent<Button>().colors = tempColB;
    }


    void UpdateUI_5_f()
    {
        //---------------------------------------------------------------------------
        tempColB = Slot3_B_GObj.GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[5] == 0)
        {
            tempColB.normalColor = colOffline;
            if (string.IsNullOrEmpty(CommunicationEvents.IPslot3))// || CommunicationEvents.IPslot3.Length < 1)
            {
                Slot3_B_GObj.GetComponentInChildren<Text>().text = "< empty >";
                //GameObject.Find("IPSlot3_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
            else
            {
                Slot3_B_GObj.GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot3 + " (Offline)";
                //GameObject.Find("IPSlot3_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot3 + " (Offline)";
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[5] == 2)
            {
                tempColB.normalColor = colOnline;
                Slot3_B_GObj.GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot3 + " (Online)";
                //GameObject.Find("IPSlot3_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot3 + " (Online)";
            }
            else
            {
                tempColB.normalColor = colOffline;
                Slot3_B_GObj.GetComponentInChildren<Text>().text = "< empty >";
                //GameObject.Find("IPSlot3_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
        }
        Slot3_B_GObj.GetComponent<Button>().colors = tempColB;
    }

 
}

















