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
using static StreamingAssetLoader;
using static CommunicationEvents;

public class StartMenue_mobile : MonoBehaviour
{


    //public int myUI_ID;
    public GameObject myself_GObj;
    //public GameObject parent_GObj;
    //public int backUI_ID;
    //public int optionsUI_ID;
    //public int failedUI_ID;
    public GameObject child1_GObj;
    public Texture2D cursorArrow_35;
    public Texture2D cursorArrow_50;
    public Texture2D cursorArrow_60;
    public Texture2D cursorArrow_70;
    public Texture2D cursorArrow_100;
    public Texture2D cursorArrow_140;
    public Texture2D cursorArrow_200;
    public Texture2D cursorArrow_300;
    public Text GObj_text;



    private void Awake()
    {
        ScreenOptimization();

        toChild1();

        if (checkOperationSystemAlreadyDone == false)
        {

            start2_CheckOS_CheckConfig();
            checkOperationSystemAlreadyDone = true;
        }
        GObj_text.text = CommunicationEvents.Opsys + "";
        CheckServerA[1] = 1;
        CheckServerA[2] = 1;
        CheckServerA[3] = 1;
    }

    void Start()
    {



       
    }

    void start2_CheckOS_CheckConfig()
    {
        switch (CommunicationEvents.Opsys)
        {
            case 0:




                if (!checkPDP())
                {

                    ResetStreamingAsset();

                    UIconfig.controlMode = CommunicationEvents.Opsys + 1;
                    NetworkJSON_Save();
                }

                NetworkJSON_Load();
                checkOS();
                ResetDataPath();
                setMouse();
                break;
            case 1:

                if (!checkPDP())
                {
                    ResetStreamingAsset();

                    UIconfig.controlMode = CommunicationEvents.Opsys + 1;
                    NetworkJSON_Save();
                }
                NetworkJSON_Load();
                checkOS();
                ResetDataPath();
                setMouse();

                break;
            default:
                setMouse();

                if (!checkPDP())
                {

                    ResetStreamingAsset();

                    UIconfig.controlMode = CommunicationEvents.Opsys + 1;
                    NetworkJSON_Save();
                }

                NetworkJSON_Load();
                checkOS();
                setMouse();
                break;
        }
    }

    private void Update()
    {
        
    }

    void checkOS()
    {
        if (CommunicationEvents.autoOSrecognition == true) {
            checkOS2();
        }
        else
        {
            //CommunicationEvents.Opsys = CommunicationEvents.Opsys_Default;
        }
        if(Opsys == 1)
        {
            ServerAutoStart = false;
        }
    }

    void checkOS2()
    {
        //https://docs.unity3d.com/ScriptReference/RuntimePlatform.html
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            
            Debug.Log("Windows OS detected");
            CommunicationEvents.Opsys = 0;
            
            return;
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("Android OS detected");
            CommunicationEvents.Opsys = 1;
            
            return;
        }


        //Default:
        //CommunicationEvents.Opsys = CommunicationEvents.Opsys_Default;
        return;
    }


    public void setMouse()
    {

        print("OPSYS: " + CommunicationEvents.Opsys);
        if (CommunicationEvents.Opsys == 1)
        {
            CommunicationEvents.CursorVisDefault = false;
            Cursor.visible = false;
        }else
        {
            Cursor.visible = true;
            CommunicationEvents.CursorVisDefault = true;
        }

        //Android crashes in level scene;
        if (CommunicationEvents.Opsys != 1) {
            double curssz = 1 / (UIconfig.cursorSize);
            // print(UIconfig.cursorSize);

            if (UIconfig.screWidth / 35 < curssz)
            {
                Cursor.SetCursor(cursorArrow_35, Vector2.zero, CursorMode.ForceSoftware);
                print("m35");
            }
            else
            {
                if (UIconfig.screWidth / 50 < curssz)
                {
                    Cursor.SetCursor(cursorArrow_50, Vector2.zero, CursorMode.ForceSoftware);
                    print("m50");
                }
                else
                {

                    if (UIconfig.screWidth / 60 < curssz)
                    {
                        Cursor.SetCursor(cursorArrow_60, Vector2.zero, CursorMode.ForceSoftware);
                        print("m60");
                    }
                    else
                    {
                        if (UIconfig.screWidth / 70 < curssz)
                        {
                            Cursor.SetCursor(cursorArrow_70, Vector2.zero, CursorMode.ForceSoftware);
                            print("m70");
                        }
                        else
                        {
                            if (UIconfig.screWidth / 100 < curssz)
                            {
                                Cursor.SetCursor(cursorArrow_100, Vector2.zero, CursorMode.ForceSoftware);
                                print("m100");
                            }
                            else
                            {
                                if (UIconfig.screWidth / 140 < curssz)
                                {
                                    Cursor.SetCursor(cursorArrow_140, Vector2.zero, CursorMode.ForceSoftware);
                                    print("m140");
                                }
                                else
                                {
                                    if (UIconfig.screWidth / 200 < curssz)
                                    {
                                        Cursor.SetCursor(cursorArrow_200, Vector2.zero, CursorMode.ForceSoftware);
                                        print("m200");
                                    }
                                    else
                                    {
                                        Cursor.SetCursor(cursorArrow_300, Vector2.zero, CursorMode.ForceSoftware);
                                        print("m300");
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }
        
        
    }


    void ScreenOptimization()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        UIconfig.screHeight = Screen.height;
        UIconfig.screWidth = Screen.width;
        CommunicationEvents.lastIP = CommunicationEvents.selecIP;


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

        UnityEngine.UI.CanvasScaler c = myself_GObj.GetComponent<UnityEngine.UI.CanvasScaler>();
        c.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;

        UIconfig.refWidth = (int)Mathf.Round(c.referenceResolution[0]);
        UIconfig.refHeight = (int)Mathf.Round(c.referenceResolution[1]);
        //CommunicationEvents.scaleMatch = 0.5f;

        /*
            //float kLogBase=10;
            //CommunicationEvents.scaleMatch = Mathf.Max(CommunicationEvents.screWidth / CommunicationEvents.refWidth, CommunicationEvents.screHeight / CommunicationEvents.refHeight);
            //float logWidth = Mathf.Log(CommunicationEvents.screWidth / CommunicationEvents.refWidth, kLogBase);
            //float logHeight = Mathf.Log(CommunicationEvents.screHeight / CommunicationEvents.refHeight, kLogBase);
            //float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, 0.5f);
            //CommunicationEvents.scaleMatch = Mathf.Pow(kLogBase, logWeightedAverage);

            //c.matchWidthOrHeight = CommunicationEvents.scaleMatch;
         /*
                RectTransform rt = GetComponent<RectTransform>();

                Vector3 screenSize = Camera.main.ViewportToWorldPoint(Vector3.up + Vector3.right);

                screenSize *= 02;

                float sizeY = screenSize.y / rt.rect.height;
                float sizeX = screenSize.x / rt.rect.width;

                rt.localScale = new Vector3(sizeX, sizeY, 1);
        */
        

    }


    public void toChild1()
    {
        ClearUIC();
        UIconfig.Andr_Start_menue_counter = 1;
        child1_GObj.SetActive(true); ;


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


