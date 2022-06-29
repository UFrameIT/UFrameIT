using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;
using static StreamingAssetLoader;
using static CheckServer;
using static CommunicationEvents;

//[RequireComponent(typeof(Image))]
//[SerializeField] private UnityEngine.UI.Image image = null;
public class FindIP : MonoBehaviour
{

    //public Button AfterIPset1; //andr
    //public Button TextSlot1;
    public GameObject backTo_GObj;
    public GameObject parentM_GObj;
    public GameObject NewServerSt_GObj;
    public GameObject InfoFindIP_GObj;
    public GameObject InfoFindIP_IP_GObj;


    public InputField mainInputField;
 
    //CheckServer CSform = new CheckServer();
    //Color colOnline = new Color(148, 229, 156, 1);
    private Color colOnline = new Color(148f / 255f, 229f / 255f, 156f / 255f, 1f);
    private Color colOffline = new Color(255f / 255f, 255f / 255f, 255f / 255f, 1f);
    private Color colPressed = new Color(133f / 255f, 140f / 255f, 107f / 255f, 1f);
    private Color colSelect = new Color(133f / 255f, 125f / 255f, 107f / 255f, 1f);
    private Color colClear = new Color(133f / 255f, 125f / 255f, 107f / 255f, 0f);
    private Color colChangeable = new Color(1f, 1f, 1f, 0.5f);
    private Color colChangeable2 = new Color(1f, 1f, 1f, 0.5f);

    private Color32 firstColB;
    private float transCol;
    private ColorBlock tempColB;









    void Start()
    {




        updateUI();











        Update();




    }


    public void updateUI()
    {
        InfoFindIP_GObj.GetComponent<Text>().text = "Enter the network address of the server you want to connect to. Verify your input with the button Select new Server. \nFor the input of the network address use the format: ";
        InfoFindIP_IP_GObj.GetComponentInChildren<Text>().text = "ipaddress:port";


    }




    void Update()
    {
        UpdateUI_2_f();
    }


    void UpdateUI_2_f()
    {


        //------------------------------------------------------------------------
        //tempColB = GameObject.Find("NewServerSt").GetComponent<Button>().colors;
        //tempColB.pressedColor = colPressed;
        //tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[2] == 0)
        {

            //tempColB.normalColor = colOffline;
            NewServerSt_GObj.GetComponent<Text>().color = Color.black;
            if (string.IsNullOrEmpty(CommunicationEvents.newIP))// || CommunicationEvents.lastIP.Length < 4)
            {
                NewServerSt_GObj.GetComponent<Text>().text = "Status: no network address";
            }
            else
            {
                NewServerSt_GObj.GetComponent<Text>().text = "Status: new server offline";
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[2] == 2)
            {

                NewServerSt_GObj.GetComponent<Text>().color = Color.green;
                NewServerSt_GObj.GetComponent<Text>().text = "Status: new server online";

            }
            else
            {
                NewServerSt_GObj.GetComponent<Text>().color = Color.black;
                if (string.IsNullOrEmpty(CommunicationEvents.newIP))// || CommunicationEvents.lastIP.Length < 4)
                {
                    NewServerSt_GObj.GetComponent<Text>().text = "Status: no network address";
                }
                else
                {

                    NewServerSt_GObj.GetComponent<Text>().color = Color.black;
                    NewServerSt_GObj.GetComponent<Text>().text = "Status: unknown";// checking network address";
                }
            }
        }
        //GameObject.Find("NewServerSt"fff).GetComponent<Button>().colors = tempColB;
        //Text txt = transform.Find("InFieldIP_Text").GetComponent<Text>();
        //txt.text = mainInputField.text;
        CommunicationEvents.newIP = mainInputField.text;

        //CommunicationEvents.newIP = transform.Find("InFieldIP_Text").GetComponent<Text>().text;
    }


    public void SetText(string text)
    {






        
            CommunicationEvents.ServerRunningA[6] = 1;
            CommunicationEvents.selecIP = CommunicationEvents.newIP;

            CommunicationEvents.IPcheckGeneration++;
        //Safefieldmenue();
        goBackButtonOPTM();






    }


    public void goBackButtonOPTM()
    {

        NetworkJSON_Save();

        ClearUIC();
        //UIconfig.Andr_Start_menue_counter = 1;
        backTo_GObj.SetActive(true); 



    }

    /// <summary>
    /// Deactivates all Pages.
    /// </summary>
    private void ClearUIC()
    {

        for (int i = 0; i < parentM_GObj.transform.childCount; i++)
        {
            parentM_GObj.transform.GetChild(i).gameObject.SetActive(false);
        }
    }
}


   
   
    




