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
    
      

    public InputField mainInputField;
    //Sprite F;

    public Canvas UIBG;
    public Canvas LaunchM;
    public Canvas OptionsM;
    public Canvas Network;
    public Canvas NetworkButtons;
    public Canvas NetworkSafe;
    public Canvas CanSlps;
    public Canvas CanS1h;
    public Canvas CanS2h;
    public Canvas CanS3h;
    public Canvas CanNSA;
    public Canvas CanInOh;
    public Canvas CanInO_Ch;
    public Canvas CanInNBh;
    public Canvas CanInNFh;
    public Canvas CanInNSh;
    public Canvas CanBG_opt1;
    public Canvas CanOverlay;
    //CheckServer CSform = new CheckServer();
    //Color colOnline = new Color(148, 229, 156, 1);
    private Color colOnline = new Color(148f/255f, 229f/255f, 156f/255f, 1f);
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
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Manue0();
        



        //tempColB = GameObject.Find("LPS").GetComponent<Button>().colors;
        //firstColB = tempColB.normalColor;
        //firstColB.a = 1; // (byte)tempColB.normalColor.a;
        //F = Resources.Load<Sprite>("Andr_Start_Manue_background.jpg");




        CommunicationEvents.screHeight = Screen.height;

        CommunicationEvents.screWidth = Screen.width;

        

        /* //ScreenMatchMode.MatchWidthOrHeight:
         // If one axis has twice resolution and the other has half, it should even out if widthOrHeight value is at 0.5.
         // In normal space the average would be (0.5 + 2) / 2 = 1.25
         // In logarithmic space the average is (-1 + 1) / 2 = 0
         float scaleFactor = Mathf.Max(screenSize.x / m_ReferenceResolution.x, screenSize.y / m_ReferenceResolution.y);

             float logWidth = Mathf.Log(screenSize.x / m_ReferenceResolution.x, kLogBase);
             float logHeight = Mathf.Log(screenSize.y / m_ReferenceResolution.y, kLogBase);
             float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, m_MatchWidthOrHeight);
             scaleFactor = Mathf.Pow(kLogBase, logWeightedAverage);


         //GameObject.Find("ASMenue").GetComponent;
         //Camera;


         //mainInputField.text = "Enter IP Here...";

         */
        UnityEngine.UI.CanvasScaler c = GameObject.Find("ASMenue").GetComponent<UnityEngine.UI.CanvasScaler>();
        c.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        
        CommunicationEvents.refWidth = (int)Mathf.Round(c.referenceResolution[0]);
        CommunicationEvents.refHeight = (int)Mathf.Round(c.referenceResolution[1]);
        //CommunicationEvents.scaleMatch = 0.5f;


        //float kLogBase=10;
        //CommunicationEvents.scaleMatch = Mathf.Max(CommunicationEvents.screWidth / CommunicationEvents.refWidth, CommunicationEvents.screHeight / CommunicationEvents.refHeight);
        //float logWidth = Mathf.Log(CommunicationEvents.screWidth / CommunicationEvents.refWidth, kLogBase);
        //float logHeight = Mathf.Log(CommunicationEvents.screHeight / CommunicationEvents.refHeight, kLogBase);
        //float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, 0.5f);
        //CommunicationEvents.scaleMatch = Mathf.Pow(kLogBase, logWeightedAverage);

        //c.matchWidthOrHeight = CommunicationEvents.scaleMatch;

        updateUI();
        CommunicationEvents.lastIP = CommunicationEvents.selecIP;



        //GameObject.Find("ASBackground").GetComponent<Image>().color.rgb = ((color.rgb - 0.5f) * _Contrast + 0.5f);
        //GetComponent(ASM_Background).sprite = F;

        //Button myBtn = go.GetComponent<Button>();


        if (!checkPDP())
        {
            ResetStreamingAsset();
            
        }
        NetworkJSON_Load();
        updateUI();

        /*
                RectTransform rt = GetComponent<RectTransform>();

                Vector3 screenSize = Camera.main.ViewportToWorldPoint(Vector3.up + Vector3.right);

                screenSize *= 02;

                float sizeY = screenSize.y / rt.rect.height;
                float sizeX = screenSize.x / rt.rect.width;

                rt.localScale = new Vector3(sizeX, sizeY, 1);
        */



        //POSITIONEN
        //Vector3 gsfg = new Vector3(1100, 11010, 0);
        //GameObject.Find("FrameWorld").transform.localPosition = gsfg;

        //GameObject.Find("FrameWorld_LM_T1").GetComponent<Image>().color = Color.black;
        //GameObject.Find("FrameWorld_LM_T1").transform.localPosition = gsfg;

        //Text txtLU = transform.Find("LUIP").GetComponent<Text>();
        //txtLU.text = "SPIELEN";






    }


    public void updateUI()
    {

        /*
         if(CommunicationEvents.lastIP.Length<4)
         {
             //GameObject.Find("LSP").SetActive(false);
             CanSlps.enabled = false;

         }
         else
         {
             //GameObject.Find("LPS").SetActive(true);
             CanSlps.enabled = true;
         }
         if (CommunicationEvents.IPslot1.Length<4)
         {
             CanS1h.enabled = false;
         }
         else
         {
             CanS1h.enabled = true;
         }
         if (CommunicationEvents.IPslot2.Length<4)
         {
             CanS2h.enabled = false;
         }
         else
         {
             CanS2h.enabled = true;
         }
         if (CommunicationEvents.IPslot3.Length<4)
         {
             CanS3h.enabled = false;
         }
         else
         {
             CanS3h.enabled = true;

         }

         */



        //Canvas.Find("IPSlot1").enabled = false;



        //GameObject.Find("InfoIP1").GetComponentInChildren<Text>().text = "Please select the IP-Address of your Server. \nFor a custom-address use format: ipaddress:port ";


        switch (CommunicationEvents.controlMode)
        {
            case 1:
               
                GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls: OFF";
                GameObject.Find("TextSlotTOO2").GetComponent<Text>().text = "Press for activating";
                break;

            case 2:
                
                GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls: ON";
                GameObject.Find("TextSlotTOO2").GetComponent<Text>().text = "Press for deactivating";
                break;
            default:
                
                GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls: OFF";
                GameObject.Find("TextSlotTOO2").GetComponent<Text>().text = "Press for activating";
                break;

        }

        switch (CommunicationEvents.touchControlMode)
        {
                case 1:
                    
                    GameObject.Find("TCMt").GetComponent<Text>().text = "Touch mode: BUTTONS";
                    GameObject.Find("TCMut").GetComponent<Text>().text = "Press for changing mode";
                    break;

                case 2:
                  

                    GameObject.Find("TCMt").GetComponent<Text>().text = "Touch mode: D-PAD";
                    GameObject.Find("TCMut").GetComponent<Text>().text = "Press for changing mode";
                    break;
            case 3:


                GameObject.Find("TCMt").GetComponent<Text>().text = "Touch mode: HYBRID";
                GameObject.Find("TCMut").GetComponent<Text>().text = "Press for changing mode";
                break;
            default:
   
                    GameObject.Find("TCMt").GetComponent<Text>().text = "Touch mode: D-PAD";
                    GameObject.Find("TCMut").GetComponent<Text>().text = "Press for changing mode";
                    break;
                      
        }

        
    GameObject.Find("TAV_Slider").GetComponent<Slider>().value = CommunicationEvents.TAvisibility;
    GameObject.Find("TAvisibilityT").GetComponent<Text>().text = "Touch area visibility " + (int)(100 * CommunicationEvents.TAvisibility) + "%";

    updateUIpreview();





    }

    void Update()
    {
        //UnityWebRequest request =  UnityWebRequest.Get(CommunicationEvents.ServerAddress2 + "/scroll/list");
        //request.method = UnityWebRequest.kHttpVerbGET;
        //yield return request.SendWebRequest();


        //if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {

        //---------------------------------------------------------------------------
        
        
        tempColB = GameObject.Find("LPS").GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[1] == 0)
        {
            
            tempColB.normalColor = colOffline;
            
            GameObject.Find("LPS").GetComponent<Button>().colors = tempColB; // new Color(148, 229, 156);
            if (string.IsNullOrEmpty(CommunicationEvents.lastIP))// || CommunicationEvents.lastIP.Length < 4)
            {
                GameObject.Find("LPS").GetComponentInChildren<Text>().text = "No game played before";
            }
            else {
                GameObject.Find("LPS").GetComponentInChildren<Text>().text = "Last played on " + CommunicationEvents.lastIP + " (Offline)";
            }
        }
        else
        {

            if (CommunicationEvents.ServerRunningA[1] == 2)
            {
                tempColB.normalColor = colOnline;
                GameObject.Find("LPS").GetComponentInChildren<Text>().text = "Last played on " + CommunicationEvents.lastIP + " (Online)";
            }
            else
            {
                tempColB.normalColor = colOffline; 
                GameObject.Find("LPS").GetComponentInChildren<Text>().text = "No game played before";
            }
        }
        GameObject.Find("LPS").GetComponent<Button>().colors = tempColB;
        
        //------------------------------------------------------------------------
        //tempColB = GameObject.Find("NewServerSt").GetComponent<Button>().colors;
        //tempColB.pressedColor = colPressed;
        //tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[2] == 0)
        {

            //tempColB.normalColor = colOffline;
            GameObject.Find("NewServerSt").GetComponent<Text>().color = Color.black;
            if (string.IsNullOrEmpty(CommunicationEvents.newIP))// || CommunicationEvents.lastIP.Length < 4)
            {
                GameObject.Find("NewServerSt").GetComponent<Text>().text = "Status: no network address";
            }
            else
            {
                GameObject.Find("NewServerSt").GetComponent<Text>().text = "Status: new server offline";
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[2] == 2)
            {
                //GameObject.Find("SelNAddrW").GetComponent<Image>().color = colOnline;
                //GameObject.Find("SelNAddrTI").GetComponent<Text>().text = CommunicationEvents.selecIP;
                GameObject.Find("NewServerSt").GetComponent<Text>().color = Color.green;
                GameObject.Find("NewServerSt").GetComponent<Text>().text = "Status: new server online";
                //GameObject.Find("SSstGame").GetComponent<Image>().color = Color.white;
                //GameObject.Find("SSstGameT").GetComponent<Text>().color = Color.black;
            }
            else
            {
                //GameObject.Find("SelNAddrW").GetComponent<Image>().color = colOffline;
                GameObject.Find("NewServerSt").GetComponent<Text>().color = Color.black;
                GameObject.Find("NewServerSt").GetComponent<Text>().text = "Status: checking network address";
                //GameObject.Find("SSstGame").GetComponent<Image>().color = colClear;
                //GameObject.Find("SSstGameT").GetComponent<Text>().color = colClear;
            }
        }
        //GameObject.Find("NewServerSt"fff).GetComponent<Button>().colors = tempColB;
        //Text txt = transform.Find("InFieldIP_Text").GetComponent<Text>();
        //txt.text = mainInputField.text;
        CommunicationEvents.newIP = mainInputField.text;

        //CommunicationEvents.newIP = transform.Find("InFieldIP_Text").GetComponent<Text>().text;



        //------------------------------------------------------------------------
        tempColB = GameObject.Find("IPSlot1").GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[3] == 0)
        {
            tempColB.normalColor = colOffline;
            if (string.IsNullOrEmpty(CommunicationEvents.IPslot1))// || CommunicationEvents.IPslot1.Length < 1)
            {
                GameObject.Find("IPSlot1").GetComponentInChildren<Text>().text = "< empty >";
                GameObject.Find("IPSlot1_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
            else
            {

                GameObject.Find("IPSlot1").GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot1 + " (Offline)";
                GameObject.Find("IPSlot1_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot1 + " (Offline)";
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[3] == 2)
            {
                tempColB.normalColor = colOnline;
                GameObject.Find("IPSlot1").GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot1 + " (Online)";
                GameObject.Find("IPSlot1_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot1 + " (Online)";
            }
            else
            {
                tempColB.normalColor = colOffline;
                GameObject.Find("IPSlot1").GetComponentInChildren<Text>().text = "< empty >";
                GameObject.Find("IPSlot1_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
        }
        GameObject.Find("IPSlot1").GetComponent<Button>().colors = tempColB;

        //-------------------------------------------------------------------------
        tempColB = GameObject.Find("IPSlot2").GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[4] == 0)
        {
            tempColB.normalColor = colOffline;
            if (string.IsNullOrEmpty(CommunicationEvents.IPslot2))// || CommunicationEvents.IPslot2.Length<1)
            {
                GameObject.Find("IPSlot2").GetComponentInChildren<Text>().text = "< empty >";
                GameObject.Find("IPSlot2_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
            else
            {
                GameObject.Find("IPSlot2").GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot2 + " (Offline)";
                GameObject.Find("IPSlot2_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot2 + " (Offline)";
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[4] == 2)
            {
                tempColB.normalColor = colOnline;
                GameObject.Find("IPSlot2").GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot2 + " (Online)";
                GameObject.Find("IPSlot2_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot2 + " (Online)";
            }
            else
            {
                tempColB.normalColor = colOffline;
                GameObject.Find("IPSlot2").GetComponentInChildren<Text>().text = "< empty >";
                GameObject.Find("IPSlot2_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
        }
        GameObject.Find("IPSlot2").GetComponent<Button>().colors = tempColB;
        
        //---------------------------------------------------------------------------
        tempColB = GameObject.Find("IPSlot3").GetComponent<Button>().colors;
        tempColB.pressedColor = colPressed;
        tempColB.selectedColor = colSelect;
        if (CommunicationEvents.ServerRunningA[5] == 0)
        {
            tempColB.normalColor = colOffline;
            if (string.IsNullOrEmpty(CommunicationEvents.IPslot3))// || CommunicationEvents.IPslot3.Length < 1)
            {
                GameObject.Find("IPSlot3").GetComponentInChildren<Text>().text = "< empty >";
                GameObject.Find("IPSlot3_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
            else
            {
                GameObject.Find("IPSlot3").GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot3 + " (Offline)";
                GameObject.Find("IPSlot3_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot3 + " (Offline)";
            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[5] == 2)
            {
                tempColB.normalColor = colOnline;
                GameObject.Find("IPSlot3").GetComponentInChildren<Text>().text = "Server " + CommunicationEvents.IPslot3 + " (Online)";
                GameObject.Find("IPSlot3_b").GetComponentInChildren<Text>().text = "OVERWRITE: \nServer " + CommunicationEvents.IPslot3 + " (Online)";
            }
            else
            {
                tempColB.normalColor = colOffline; 
                GameObject.Find("IPSlot3").GetComponentInChildren<Text>().text = "< empty >";
                GameObject.Find("IPSlot3_b").GetComponentInChildren<Text>().text = "OVERWRITE: \n< empty >";
            }
        }
        GameObject.Find("IPSlot3").GetComponent<Button>().colors = tempColB;
        
        //--------------------------------------------------------------------------
        
        if (CommunicationEvents.ServerRunningA[6] == 0 || CommunicationEvents.ServerRunningA[6] ==3)
        {
            GameObject.Find("SelNAddrW").GetComponent<Image>().color = colOffline;
            GameObject.Find("SSstGame").GetComponent<Image>().color = colClear;
            GameObject.Find("SSstGameT").GetComponent<Text>().color = colClear;
            GameObject.Find("StartGwoS").GetComponent<Image>().color = colClear;
            GameObject.Find("SGwoST").GetComponent<Text>().color = colClear;
            if (string.IsNullOrEmpty(CommunicationEvents.selecIP))//; || ())
            {
                GameObject.Find("SelNAddrTI").GetComponent<Text>().text = "< empty >";
                GameObject.Find("newNAddrTI").GetComponent<Text>().text = "< empty >";
                GameObject.Find("SelNAddrTS").GetComponent<Text>().text = "Status: no server";
                //GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: no server";
                //GameObject.Find("SelNAddrTS").GetComponent<Text>().text = "Status: no ip-address";
                GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: no ip-address";
            }
            else
            {
                GameObject.Find("SelNAddrTI").GetComponent<Text>().text = CommunicationEvents.selecIP;
                GameObject.Find("newNAddrTI").GetComponent<Text>().text = CommunicationEvents.selecIP;

                if (CommunicationEvents.ServerRunningA[6] == 0)
                { 
                    GameObject.Find("SelNAddrTS").GetComponent<Text>().text = "Status: Offline";   
                    GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: Offline"; 

                }
                else
                {
                    GameObject.Find("SelNAddrTS").GetComponent<Text>().text = "Status: no ip-address";
                    GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: no ip-address";
                }

            }
        }
        else
        {
            if (CommunicationEvents.ServerRunningA[6] == 2)
            {
                GameObject.Find("SelNAddrW").GetComponent<Image>().color = colOnline;
                GameObject.Find("SelNAddrTI").GetComponent<Text>().text = CommunicationEvents.selecIP;
                GameObject.Find("SelNAddrTS").GetComponent<Text>().text = "Status: Online";
                GameObject.Find("SSstGame").GetComponent<Image>().color = Color.white;
                GameObject.Find("SSstGameT").GetComponent<Text>().color = Color.black;
                GameObject.Find("StartGwoS").GetComponent<Image>().color = Color.white;
                GameObject.Find("SGwoST").GetComponent<Text>().color = Color.black;
                GameObject.Find("newNAddrTI").GetComponent<Text>().text = CommunicationEvents.selecIP;
                GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: Online";
            }
            else
            {
                GameObject.Find("SelNAddrW").GetComponent<Image>().color = colOffline;
                GameObject.Find("SelNAddrTI").GetComponent<Text>().text = CommunicationEvents.selecIP;
                
                GameObject.Find("SSstGame").GetComponent<Image>().color = colClear;
                GameObject.Find("SSstGameT").GetComponent<Text>().color = colClear;
                GameObject.Find("StartGwoS").GetComponent<Image>().color = colClear;
                GameObject.Find("SGwoST").GetComponent<Text>().color = colClear; 
                GameObject.Find("newNAddrTI").GetComponent<Text>().text = CommunicationEvents.selecIP;
                


                GameObject.Find("SelNAddrTS").GetComponent<Text>().text = "Status: checking";
                GameObject.Find("newNAddrTS").GetComponent<Text>().text = "Status: checking";
                

            }
        }

        //------------------------------------------------------------------------------
        //GameObject.Find("ResetASD_but").GetComponentInChildren<Text>().text = CommunicationEvents.controlmode.ToString();

        
    }

    public void updateUIpreview()
    {

        if (CommunicationEvents.controlMode==2) {

            colChangeable.a = CommunicationEvents.TAvisibility;
            colChangeable2.a = (CommunicationEvents.TAvisibility)/5;
            switch (CommunicationEvents.touchControlMode)
            {
                case 1:
                    GameObject.Find("ExBttnPreview1").GetComponentInChildren<Image>().color = colChangeable;
                    GameObject.Find("ExBttnPreview2l").GetComponentInChildren<Image>().color = colChangeable;
                    GameObject.Find("ExBttnPreview2r").GetComponentInChildren<Image>().color = colChangeable;
                    GameObject.Find("ExDpadPreview1").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExDpadPreview2").GetComponentInChildren<Image>().color = colClear;
                    break;
                case 2:
                    GameObject.Find("ExBttnPreview1").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExBttnPreview2l").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExBttnPreview2r").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExDpadPreview1").GetComponentInChildren<Image>().color = colChangeable2;
                    GameObject.Find("ExDpadPreview2").GetComponentInChildren<Image>().color = colChangeable2;
                    break;
                case 3:
                    GameObject.Find("ExBttnPreview1").GetComponentInChildren<Image>().color = colChangeable;
                    GameObject.Find("ExBttnPreview2l").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExBttnPreview2r").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExDpadPreview1").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExDpadPreview2").GetComponentInChildren<Image>().color = colChangeable2;
                    break;



                default:
                    GameObject.Find("ExBttnPreview1").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExBttnPreview2l").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExBttnPreview2r").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExDpadPreview1").GetComponentInChildren<Image>().color = colClear;
                    GameObject.Find("ExDpadPreview2").GetComponentInChildren<Image>().color = colClear;
                    break;
            }
        }
        else
        {
            GameObject.Find("ExBttnPreview1").GetComponentInChildren<Image>().color = colClear;
            GameObject.Find("ExBttnPreview2l").GetComponentInChildren<Image>().color = colClear;
            GameObject.Find("ExBttnPreview2r").GetComponentInChildren<Image>().color = colClear;
            GameObject.Find("ExDpadPreview1").GetComponentInChildren<Image>().color = colClear;
            GameObject.Find("ExDpadPreview2").GetComponentInChildren<Image>().color = colClear;
        }





    }



    public void SetText(string text)
	{




        //(CommunicationEvents.ServerAdress = "http://" + mainInputField.text;
        //SceneManager.LoadScene("Andr_TreeWorld");
        //SceneManager.LoadScene("MainMenue");

        /*
        UIBG.enabled = true;
        LaunchM.enabled = false;
        NetworkButtons.enabled = false;
        Network.enabled = false;
        NetworkSafe.enabled = true;
        */
        CommunicationEvents.ServerRunningA[6] = 1;
        CommunicationEvents.selecIP = CommunicationEvents.newIP;
        
        CommunicationEvents.IPcheckGeneration++;
        Safefieldmenue();




        //GameObject.Find("AfterIPset1").GetComponentInChildren<Text>().text = "hey";


    }




    public void _TestInput() 
   {

		//CommunicationEvents.ServerAdress = mainInputField.text;       
		//or
		//string text;

		Score_Save("test","sesam");


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

    public void Slot1_s()
    {
        CommunicationEvents.IPslot1 = CommunicationEvents.newIP;
        //CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress1;
        updateUI();
        
        NetworkJSON_Save();
        
        //CSform.CheckIPAdr();

    }

    public void Slot2_s()
    {

        //CommunicationEvents.ServerAdress = "http://" +  CommunicationEvents.ServerAddress2;
        CommunicationEvents.IPslot2 = CommunicationEvents.newIP;
        updateUI();

        NetworkJSON_Save();
        //CSform.CheckIPAdr();


    }

    public void Slot3_s()
    {
        //CheckServer ani = new CheckServer();//= obj.AddComponent<CheckServer>();
        // ani.StartCheck();
        CommunicationEvents.IPslot3 = CommunicationEvents.newIP;
        updateUI();

        NetworkJSON_Save();
        //CSform.CheckIPAdr();


    }



    public void LastplayedButton()
    {

        //CommunicationEvents.ServerAdress = "http://" + CommunicationEvents.ServerAddress2;
        //SceneManager.LoadScene("Andr_TreeWorld");       
        //SceneManager.LoadScene("MainMenue");

        //Text txt = transform.Find("LUIP").GetComponent<Text>();
        //txt.text = Score_Load("test");
        //sw = Screen.width;
        //UnityEngine.UI.CanvasScaler c = GameObject.Find("ASMenue").GetComponent<UnityEngine.UI.CanvasScaler>();
        //float a = //c.referenceResolution[1];
        //sw = (int)Mathf.Round(a);

        //txt.text = 
        //CommunicationEvents.scaleMatch.ToString();

        //if (string.IsNullOrEmpty(CommunicationEvents.lastIP))
        if (CommunicationEvents.ServerRunningA[6]==2)
        {
            startNextSceneFunctionNewGame();
        }
        else
        {
            //GameObject.Find("LSP").SetActive(false);
            ChangeIPMButton();
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
            Safefieldmenue();
        }
    }

    public void startNextSceneFunctionNewGame()
    {
        NetworkJSON_Save();
        CommunicationEvents.ServerAdress = "http://" + CommunicationEvents.selecIP;
        SceneManager.LoadScene("MainMenue");
    }

    public string Score_Load(string Directory_path)
    {
        //Data acquisition
        //var reader = new StreamReader(Application.persistentDataPath + "/" + Directory_path + "/date.json");
        //var reader = new StreamReader(Application.persistentDataPath + "/scrolls.json");
        //var reader = new StreamReader(Application.persistentDataPath + "/1/scrolls.json");
        //var reader = new StreamReader(Application.persistentDataPath + "/test3/test7.json");
        //var reader = new StreamReader(Application.persistentDataPath + "Stages/factStateMAchines/TechDemo B_val.JSON");
        //var reader = new StreamReader(Application.persistentDataPath + "/Stages/TechDemo B.JSON");
        var reader = new StreamReader(Application.persistentDataPath + "/Config/Network.JSON");
        string json = reader.ReadToEnd();
        reader.Close();

        //MyClass myObjs = JsonUtility.FromJson<MyClass>(json);

        //SampleData mySampleFile = JsonUtility.FromJson<SampleData>(jsonStr);
        return json;//Convert for ease of use
        //return myObjs.level.ToString();
    }



    public void goBackButtonOPTM()
    {

        NetworkJSON_Save();
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

        }
    }

    public void TouchControls()
    {
        switch (CommunicationEvents.controlMode)
        {
            case 1:
                CommunicationEvents.controlMode=2;
                GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls: ON";
                GameObject.Find("TextSlotTOO2").GetComponent<Text>().text = "Press for deactivating";
                break;

            case 2:
                CommunicationEvents.controlMode=1;
                //GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls OFF";
                GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls: OFF";
                GameObject.Find("TextSlotTOO2").GetComponent<Text>().text = "Press for activating";
                break;
            default:
                CommunicationEvents.controlMode = 2;
                GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls: ON";
                GameObject.Find("TextSlotTOO2").GetComponent<Text>().text = "Press for deactivating";
                break;

        }
        updateUIpreview();
        NetworkJSON_Save();
    }

    public void TouchControlModes()
    {
        switch (CommunicationEvents.touchControlMode)
        {
            case 1:
                CommunicationEvents.touchControlMode = 2;
                GameObject.Find("TCMt").GetComponent<Text>().text = "Touch mode: D-PAD";
                GameObject.Find("TCMut").GetComponent<Text>().text = "Press for changing mode";
                break;

            case 2:
                CommunicationEvents.touchControlMode = 3;
                //GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls OFF";
                GameObject.Find("TCMt").GetComponent<Text>().text = "Touch mode: HYBRID";
                GameObject.Find("TCMut").GetComponent<Text>().text = "Press for changing mode";
                break;
            case 3:
                CommunicationEvents.touchControlMode = 1;
                //GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls OFF";
                GameObject.Find("TCMt").GetComponent<Text>().text = "Touch mode: BUTTONS";
                GameObject.Find("TCMut").GetComponent<Text>().text = "Press for changing mode";
                break;
            default:
                CommunicationEvents.touchControlMode = 1;
                GameObject.Find("TCMt").GetComponent<Text>().text = "Touch mode: BUTTONS";
                GameObject.Find("TCMut").GetComponent<Text>().text = "Press for changing mode";
                break;

        }
        updateUIpreview();
        NetworkJSON_Save();
    }

    public void touchAreaVisibilityBttn()
    {
        CommunicationEvents.TAvisibility = GameObject.Find("TAV_Slider").GetComponent<Slider>().value;
        GameObject.Find("TAvisibilityT").GetComponent<Text>().text = "Touch area visibility " + (int)(100*CommunicationEvents.TAvisibility) + "%";

        updateUIpreview();

    }




        public void Manue0()
    {
        UIBG.enabled = true;
        LaunchM.enabled = true;
        NetworkButtons.enabled = false;
        Network.enabled = false;
        NetworkSafe.enabled = false;
        OptionsM.enabled = false;
        CanSlps.enabled = false;
        CanS1h.enabled = false;
        CanS2h.enabled = false;
        CanS3h.enabled = false;
        CanInOh.enabled = false;
        CanInNBh.enabled = false;
        CanInNFh.enabled = false;
        CanInNSh.enabled = false;
        CanBG_opt1.enabled = false;
        CanOverlay.enabled = false;
        CommunicationEvents.Andr_Start_menue_counter = 1;
        CanNSA.enabled = false;



    }

    public void OptionsMB()
    {
        UIBG.enabled = true;
        LaunchM.enabled = false;
        NetworkButtons.enabled = false;
        Network.enabled = false;
        NetworkSafe.enabled = false;
        OptionsM.enabled = true;
        CanSlps.enabled = false;
        CanS1h.enabled = false;
        CanS2h.enabled = false;
        CanS3h.enabled = false;
        CanInOh.enabled = true;
        CanInO_Ch.enabled = false;
        CanInNBh.enabled = false;
        CanInNFh.enabled = false;
        CanInNSh.enabled = false;
        CanBG_opt1.enabled = true;
        CanOverlay.enabled = true;
        CommunicationEvents.Andr_Start_menue_counter = 2;
        CanNSA.enabled = false;
    }

    public void OptionsCntrlMB()
    {
        UIBG.enabled = true;
        LaunchM.enabled = false;
        NetworkButtons.enabled = false;
        Network.enabled = false;
        NetworkSafe.enabled = false;
        OptionsM.enabled = true;
        CanSlps.enabled = false;
        CanS1h.enabled = false;
        CanS2h.enabled = false;
        CanS3h.enabled = false;
        CanInOh.enabled = false;
        CanInO_Ch.enabled = true;
        CanInNBh.enabled = false;
        CanInNFh.enabled = false;
        CanInNSh.enabled = false;
        CanBG_opt1.enabled = true;
        CanOverlay.enabled = true;
        CommunicationEvents.Andr_Start_menue_counter = 6;
        CanNSA.enabled = false;
    }

    public void ChangeIPMButton()
    {

        LaunchM.enabled = false;
        NetworkButtons.enabled = true;
        Network.enabled = false;
        NetworkSafe.enabled = false;
        OptionsM.enabled = false;
        CanSlps.enabled = true;
        CanS1h.enabled = true;
        CanS2h.enabled = true;
        CanS3h.enabled = true;
        CanInOh.enabled = false;
        CanInO_Ch.enabled = false;
        CanInNBh.enabled = true;
        CanInNFh.enabled = false;
        CanInNSh.enabled = false;
        CanBG_opt1.enabled = true;
        CanOverlay.enabled = true;
        CommunicationEvents.Andr_Start_menue_counter = 3;
        CanNSA.enabled = true;

    }

    public void Inputfieldmenue()
    {
        UIBG.enabled = true;
        LaunchM.enabled = false;
        NetworkButtons.enabled = false;
        Network.enabled = true;
        NetworkSafe.enabled = false;
        OptionsM.enabled = false;
        CanSlps.enabled = false;
        CanS1h.enabled = false;
        CanS2h.enabled = false;
        CanS3h.enabled = false;
        CanInOh.enabled = false;
        CanInO_Ch.enabled = false;
        CanInNBh.enabled = false;
        CanInNFh.enabled = true;
        CanInNSh.enabled = false;
        CanBG_opt1.enabled = true;
        CanOverlay.enabled = true;
        CommunicationEvents.Andr_Start_menue_counter = 4;
        CanNSA.enabled = false;



    }


    public void Safefieldmenue()
    {
        UIBG.enabled = true;
        LaunchM.enabled = false;
        NetworkButtons.enabled = false;
        Network.enabled = false;
        NetworkSafe.enabled = true;
        OptionsM.enabled = false;
        CanSlps.enabled = false;
        CanS1h.enabled = false;
        CanS2h.enabled = false;
        CanS3h.enabled = false;
        CanInOh.enabled = false;
        CanInO_Ch.enabled = false;
        CanInNBh.enabled = false;
        CanInNFh.enabled = false;
        CanInNSh.enabled = true;
        CanBG_opt1.enabled = true;
        CanOverlay.enabled = true;
        CommunicationEvents.Andr_Start_menue_counter = 5;
        CanNSA.enabled = false;



    }

    public void ResetStreamingAsset()
    {
        RereadFileUWR(StreamingAssetLoader.file_1_path, StreamingAssetLoader.file_1);
        RereadFileUWR(StreamingAssetLoader.file_2_path, StreamingAssetLoader.file_2);
        RereadFileUWR(StreamingAssetLoader.file_3_path, StreamingAssetLoader.file_3);
        RereadFileUWR(StreamingAssetLoader.file_4_path, StreamingAssetLoader.file_4);
        RereadFileUWR(StreamingAssetLoader.file_5_path, StreamingAssetLoader.file_5);
        RereadFileUWR(StreamingAssetLoader.file_6_path, StreamingAssetLoader.file_6);
        RereadFileUWR(StreamingAssetLoader.file_7_path, StreamingAssetLoader.file_7);
        RereadFileUWR(StreamingAssetLoader.file_8_path, StreamingAssetLoader.file_8);
        RereadFileUWR(StreamingAssetLoader.file_9_path, StreamingAssetLoader.file_9);
        RereadFileUWR(StreamingAssetLoader.file_10_path, StreamingAssetLoader.file_10);
        NetworkJSON_Load();
        updateUI();
        //CSform.CheckIPAdr();
    }







    public static DirectoryInfo SafeCreateDirectory(string path)
    {
        //Generate if you don't check if the directory exists
        if (Directory.Exists(path))
        {
            return null;
        }
        return Directory.CreateDirectory(path);
    }

	public class MyClass
	{
		public int level;
		public float timeElapsed;
		public string playerName;
	}

    public class NetworkJSON
    {
        public string lastIP;
        public string newIP;
        public string IPslot1;
        public string IPslot2;
        public string IPslot3;
        public string selecIP;
        public int ControlMode;
        public int TouchMode;
        public float TAvisibility;
    }

    public void Score_Save(string Directory_path, string date)
    {
		MyClass myObject = new MyClass();
		myObject.level = 1;
		myObject.timeElapsed = 47.5f;
		myObject.playerName = "Dr Charles Francis";

		//Data storage
		SafeCreateDirectory(Application.persistentDataPath + "/" + Directory_path);
		//string json = JsonUtility.ToJson(date);
		string json = JsonUtility.ToJson(myObject);
		StreamWriter Writer = new StreamWriter(Application.persistentDataPath + "/" + Directory_path + "/date.json");
        Writer.Write(json);
        Writer.Flush();
        Writer.Close();



        //RereadFileUW("", "scrolls.json", "test3", "test6.json");
        //RereadFileUW("Stages", "TechDemo A.JSON", "test3", "test7.json");

        /*
        RereadFileUWR("", "scrolls.json");
        RereadFileUWR("Stages", "TechDemo A.JSON");
        RereadFileUWR("Stages", "TechDemo B.JSON");
        RereadFileUWR("Stages/ValidationSets", "TechDemo A_val.JSON");
        RereadFileUWR("Stages/ValidationSets", "TechDemo B_val.JSON");
        RereadFileUWR("Stages/ValidationSets/FactStateMachines", "TechDemo A_val.JSON");
        RereadFileUWR("Stages/ValidationSets/FactStateMachines", "TechDemo B_val.JSON");
        */



    }

    public void NetworkJSON_Save()
    {
        NetworkJSON myObject = new NetworkJSON();
        //MyClass myObject = new MyClass();
        myObject.newIP = CommunicationEvents.newIP;
        myObject.lastIP = CommunicationEvents.lastIP;
        myObject.IPslot1 = CommunicationEvents.IPslot1;
        myObject.IPslot2 = CommunicationEvents.IPslot2;
        myObject.IPslot3 = CommunicationEvents.IPslot3;
        myObject.selecIP = CommunicationEvents.selecIP;
        myObject.ControlMode = CommunicationEvents.controlMode;
        myObject.TouchMode = CommunicationEvents.touchControlMode;
        myObject.TAvisibility = CommunicationEvents.TAvisibility;

        //Data storage
        SafeCreateDirectory(Application.persistentDataPath + "/Config");
        //string json = JsonUtility.ToJson(date);
        string json = JsonUtility.ToJson(myObject);
        StreamWriter Writer = new StreamWriter(Application.persistentDataPath + "/Config/Network.json");
        Writer.Write(json);
        Writer.Flush();
        Writer.Close();
    }

    public void NetworkJSON_Load()
    {
        var reader = new StreamReader(Application.persistentDataPath + "/Config/Network.JSON");
        string json = reader.ReadToEnd();
        reader.Close();

        NetworkJSON myObjs = JsonUtility.FromJson<NetworkJSON>(json);
        if (string.IsNullOrEmpty(myObjs.newIP))
        {
            CommunicationEvents.newIP = "";
        }
        else
        {
            CommunicationEvents.newIP = myObjs.newIP;
        }
        if (string.IsNullOrEmpty(myObjs.lastIP))
        {
            CommunicationEvents.lastIP = "";
        }
        else
        {
            CommunicationEvents.lastIP = myObjs.lastIP;
        }
        if (string.IsNullOrEmpty(myObjs.IPslot1))
        {
            CommunicationEvents.IPslot1 = "";
        }
        else
        {
            CommunicationEvents.IPslot1 = myObjs.IPslot1 ;//myObjs.IPslot1;
        }
        if (string.IsNullOrEmpty(myObjs.IPslot2))
        {
            CommunicationEvents.IPslot2 = "";//"Empty";
        }
        else
        {
            CommunicationEvents.IPslot2 = myObjs.IPslot2;
        }
        if (string.IsNullOrEmpty(myObjs.IPslot3))
        {
            CommunicationEvents.IPslot3 = ""; 
        }
        else
        {
            CommunicationEvents.IPslot3 = myObjs.IPslot3;
        }
        if (string.IsNullOrEmpty(myObjs.selecIP))
        {
            CommunicationEvents.selecIP = "";
        }
        else
        {
            CommunicationEvents.selecIP = myObjs.selecIP;
        }
        if (false)
        {
            
        }
        else
        {
            CommunicationEvents.controlMode = myObjs.ControlMode;
        }
        if (false)
        {

        }
        else
        {
            CommunicationEvents.touchControlMode = myObjs.TouchMode;
        }
        if (false)
        {

        }
        else
        {
            CommunicationEvents.TAvisibility = myObjs.TAvisibility;
        }

        





    }




    public static string RereadFileNA(string pathfolders, string fileName, string destpathf,  string destname)
    {
        if (fileName == "")
        {
            return "noName";
        }




         // copies and unpacks file from apk to persistentDataPath where it can be accessed
        string destinationPath = Path.Combine(Application.persistentDataPath, destpathf);

        if (Directory.Exists(destinationPath) == false)
        {
            Directory.CreateDirectory(destinationPath);
        }


        destinationPath = Path.Combine(destinationPath, destname);




        #if UNITY_EDITOR
                string sourcePath = Path.Combine(Application.streamingAssetsPath, pathfolders);
                sourcePath = Path.Combine(sourcePath, fileName);
        #else
                string sourcePath = "jar:file://" + Application.dataPath + "!/assets/" + fileName;

        #endif

        //UnityEngine.Debug.Log(string.Format("{0}-{1}-{2}-{3}", sourcePath,  File.GetLastWriteTimeUtc(sourcePath), File.GetLastWriteTimeUtc(destinationPath)));

        //copy whatsoever

        //if DB does not exist in persistent data folder (folder "Documents" on iOS) or source DB is newer then copy it
        //if (!File.Exists(destinationPath) || (File.GetLastWriteTimeUtc(sourcePath) > File.GetLastWriteTimeUtc(destinationPath)))
        {
            if (sourcePath.Contains("://"))
            {
                // Android  
                WWW www = new WWW(sourcePath);
                while (!www.isDone) {; }                // Wait for download to complete - not pretty at all but easy hack for now 
                if (string.IsNullOrEmpty(www.error))
                {
                    File.WriteAllBytes(destinationPath, www.bytes);
                }
                else
                {
                    Debug.Log("ERROR: the file DB named " + fileName + " doesn't exist in the StreamingAssets Folder, please copy it there.");
                }
            }
            else
            {
                // Mac, Windows, Iphone                
                //validate the existens of the DB in the original folder (folder "streamingAssets")
                if (File.Exists(sourcePath))
                {
                    //copy file - alle systems except Android
                    File.Copy(sourcePath, destinationPath, true);
                }
                else
                {
                    Debug.Log("ERROR: the file DB named " + fileName + " doesn't exist in the StreamingAssets Folder, please copy it there.");
                }
            }
        }

        StreamReader reader = new StreamReader(destinationPath);
        var jsonString = reader.ReadToEnd();
        reader.Close();


        return jsonString;
    }

    public static void RereadFileUWR(string pathfolders, string fileName)
    {

        if (fileName == "")
        {
            return;
        }
        string destpathf = pathfolders;
        string destname = fileName;


        string sourcePath = Path.Combine(Application.streamingAssetsPath, pathfolders);
        sourcePath = Path.Combine(sourcePath, fileName);
        var loadingRequest = UnityWebRequest.Get(sourcePath);
        loadingRequest.SendWebRequest();
        while (!loadingRequest.isDone)
        {
            if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
            {
                break;
            }
        }
        if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
        {

        }
        else
        {
            //copies and unpacks file from apk to persistentDataPath where it can be accessed
            string destinationPath = Path.Combine(Application.persistentDataPath, destpathf);

            if (Directory.Exists(destinationPath) == false)
            {
                Directory.CreateDirectory(destinationPath);
            }
            File.WriteAllBytes(Path.Combine(destinationPath, destname), loadingRequest.downloadHandler.data);
        }



        }
    public static void RereadFileUW4(string pathfolders, string fileName, string destpathf, string destname)
    {
        if (fileName == "")
        {
            return;
        }


        string sourcePath = Path.Combine(Application.streamingAssetsPath, pathfolders);
        sourcePath = Path.Combine(sourcePath, fileName);
        var loadingRequest = UnityWebRequest.Get(sourcePath);
        loadingRequest.SendWebRequest();
        while (!loadingRequest.isDone)
        {
            if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
            {
                break;
            }
        }
        if (loadingRequest.isNetworkError || loadingRequest.isHttpError)
        {

        }
        else
        {
            //copies and unpacks file from apk to persistentDataPath where it can be accessed
            string destinationPath = Path.Combine(Application.persistentDataPath, destpathf);

            if (Directory.Exists(destinationPath) == false)
            {
                Directory.CreateDirectory(destinationPath);
            }
            File.WriteAllBytes(Path.Combine(destinationPath, destname), loadingRequest.downloadHandler.data);
        }




    }

    public bool checkPDP()
    {
        
        string filePath = Application.persistentDataPath + "/Config/Network.json";
        if (System.IO.File.Exists(filePath) )
        {
            return true;
        }
        
        return false;
    }
}



