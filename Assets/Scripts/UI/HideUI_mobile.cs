﻿    using UnityEngine;
using UnityEngine.SceneManagement;
using static CommunicationEvents;
using static UIconfig;
using static Restart;

public class HideUI_mobile : MonoBehaviour
{

    public KeyCode Key = KeyCode.F1;
    public KeyCode visMouse = KeyCode.LeftControl;
    //public KeyCode ScreenshotKey = KeyCode.F12;

    public string
        modifier,
        modundo,
        modredo,
        modreset,
        modsave,
        modload;
        
    public string toolMode_keyBind;
    public string MathMode_keyBind;
    public string cancel_keyBind;

    //public UnityStandardAssets.Characters.FirstPerson.FirstPersonController CamControl;
    public Characters.FirstPerson.FirstPersonController1 CamControl;
    public bool LockOnly = true;
    public MeshRenderer CursorRenderer;
    internal Canvas UICanvas;



    void Start()
    {
        if (UIconfig.FrameITUIversion == 2) // 1= FrameITUI; 2= FrameITUI_mobil
        {
            Start2();
        }
        else
        {
            if (UICanvas == null)
            {
                UICanvas = GetComponentInChildren<Canvas>();
            }
           UICanvas.enabled = false;
        }


    }


    void Start2()
    {

        if (!LockOnly)
        {
            if (UICanvas == null)
            {
                UICanvas = GetComponentInChildren<Canvas>();
            }
            bool camActive;
            camActive = false;
            UICanvas.enabled = camActive;
            CamControl.enabled = false;
            CursorRenderer.enabled = true;

        }
        //}
        //Start3();
        //CamControl.enabled = true;

    }

    void Start3()
    {
        print("Start3");
        UIconfig.CanvasOnOff_Array[14] = 1;
        UIconfig.CanvasOnOff_Array[20] = 0;
        //setUI_Vis_walk(0);
        //UIconfig.CanvasOnOff_Array[20] = 0;
        Update();
        //CheckUI_Vis_walk();
        UIconfig.CanvasOnOff_Array[14] = 0;
        UIconfig.CanvasOnOff_Array[20] = 1;
        UIconfig.CanvasOnOff_Array[10] = 1;
        
        UIconfig.CanvasOnOff_Array[3] = 1;
        CamControl.enabled = true;
        CamControl.enabled = false;

        Update();
        print("Start4");


    }

    // Update is called once per frame
    void Update()
    {
        if (UIconfig.FrameITUIversion == 2)
        {
            Update3();
        }
        print("dada" + UIconfig.CanvasOnOff_Array[4]);
    }


    public void HudButton()
    {
        Update3();
    }


    void Update3()
    {
        CheckUI_Vis_walk();
        CheckIf();
        Update2();
        CheckUI_Vis();
        
    }

    void CheckUI_Vis_walk()
    {
        int uiccm = UIconfig.controlMode - 1;
        UIconfig.CanvasOnOff_Array[11] = uiccm;
        UIconfig.CanvasOnOff_Array[12] = uiccm;
        UIconfig.CanvasOnOff_Array[13] = uiccm;
        UIconfig.CanvasOnOff_Array[15] = uiccm;
        UIconfig.CanvasOnOff_Array[17] = uiccm;
        UIconfig.CanvasOnOff_Array[18] = uiccm;
    }
    void setUI_Vis_walk(int a)
    {
        int uiccm = a;
        UIconfig.CanvasOnOff_Array[11] = uiccm;
        UIconfig.CanvasOnOff_Array[12] = uiccm;
        UIconfig.CanvasOnOff_Array[13] = uiccm;
        UIconfig.CanvasOnOff_Array[15] = uiccm;
        UIconfig.CanvasOnOff_Array[17] = uiccm;
        UIconfig.CanvasOnOff_Array[18] = uiccm;
    }




    void CheckIf()
    {

        

        //walking
        if (UIconfig.CanvasOnOff_Array[10] == 1 && UIconfig.CanvasOnOff_Array[20] == 1 && UIconfig.CanvasOnOff_Array[14] == 0)
        {
            
                if (Input.GetButtonDown(toolMode_keyBind))
                {
                    //print("w2");

                    UIconfig.CanvasOnOff_Array[14] = 1;
                    UIconfig.CanvasOnOff_Array[20] = 0;

                return;
                }


                if (Input.GetButtonDown(MathMode_keyBind))
                {

                    UIconfig.CanvasOnOff_Array[16] = 1;
                    UIconfig.CanvasOnOff_Array[20] = 0;

                return;
                }

                if (Input.GetButtonDown(cancel_keyBind))
                {

                    UIconfig.CanvasOnOff_Array[02] = 1;
                    UIconfig.CanvasOnOff_Array[10] = 0;


                    return;
                }
                return;
            
        }

        
        //Toolmode
        if (UIconfig.CanvasOnOff_Array[10] == 1 && UIconfig.CanvasOnOff_Array[20] == 0 && UIconfig.CanvasOnOff_Array[14] == 1)
        {
            //print("t1");
            if (Input.GetButtonDown(toolMode_keyBind))
            {
                //print("t2");

                UIconfig.CanvasOnOff_Array[14] = 0;
                UIconfig.CanvasOnOff_Array[20] = 1;
                return;

            }
            if (Input.GetButtonDown(MathMode_keyBind))
            {
   
                UIconfig.CanvasOnOff_Array[14] = 0;
                UIconfig.CanvasOnOff_Array[16] = 1;
                return;
            }
            if (Input.GetButtonDown(cancel_keyBind))
            {

                UIconfig.CanvasOnOff_Array[02] = 1;
                UIconfig.CanvasOnOff_Array[10] = 0;


                return;
            }
            return;
        }
        //PauseMenue

        //MathMenue
        if (Input.GetButtonDown(MathMode_keyBind))
        {

            UIconfig.CanvasOnOff_Array[16] = 0;
            UIconfig.CanvasOnOff_Array[20] = 1;
            return;
        }
        if (Input.GetButtonDown(cancel_keyBind))
        {

            UIconfig.CanvasOnOff_Array[02] = 1;
            UIconfig.CanvasOnOff_Array[10] = 0;


            return;
        }
        return;
    }









    void CheckUI_Vis() 
    {
        //Toolmode
        if (UIconfig.CanvasOnOff_Array[14] == 1 && UIconfig.CanvasOnOff_Array[10] == 1)
        {
            if (LockOnly)
            {
                CamControl.enabled = !CamControl.enabled;
                CursorRenderer.enabled = CamControl.enabled;
            }
            else
            {
                Cursor.visible = true;
                CamControl.enabled = false;
                CursorRenderer.enabled = true;

                UICanvas.enabled = false;
            }
            return;

        }

        //Walkingmode
        if (UIconfig.CanvasOnOff_Array[20] == 1 && UIconfig.CanvasOnOff_Array[10] == 1)
        {
            if (LockOnly)
            {
                CamControl.enabled = !CamControl.enabled;
                CursorRenderer.enabled = CamControl.enabled;
            }
            else
            {
                Cursor.visible = false;
                CamControl.enabled = true;
                CursorRenderer.enabled = false;

                UICanvas.enabled = false;
            }
            return;

        }
        //Mathmode
        if (UIconfig.CanvasOnOff_Array[16] == 1 && UIconfig.CanvasOnOff_Array[10] == 1)
        {
            if (LockOnly)
            {
                CamControl.enabled = !CamControl.enabled;
                CursorRenderer.enabled = CamControl.enabled;
            }
            else
            {
                  
                Cursor.visible = true;
                CamControl.enabled = false;
                CursorRenderer.enabled = false;
                UICanvas.enabled = true;


            }
            return;
        }
        //PauseMenue
        if (UIconfig.CanvasOnOff_Array[2] == 1)
        {
            if (LockOnly)
            {
                CamControl.enabled = !CamControl.enabled;
                CursorRenderer.enabled = CamControl.enabled;
            }
            else
            {
                
                Cursor.visible = true;
                CamControl.enabled = false;
                CursorRenderer.enabled = false;
                UICanvas.enabled = false;
            }
            return;

        }
        //Startmenue
        if (UIconfig.CanvasOnOff_Array[3] == 1)
        {
            if (LockOnly)
            {
                CamControl.enabled = !CamControl.enabled;
                CursorRenderer.enabled = CamControl.enabled;
            }
            else
            {
                UICanvas.enabled = false;
                Cursor.visible = true;
                CamControl.enabled = false;
                CursorRenderer.enabled = false;
                
                
            }
            return;

        }
        if (UIconfig.CanvasOnOff_Array[20] != 1 && UIconfig.CanvasOnOff_Array[14] != 1 && UIconfig.CanvasOnOff_Array[16] != 1)
        {
            print("CheckHideUI_mobile");
            
            if (LockOnly)
            {
                CamControl.enabled = !CamControl.enabled;
                CursorRenderer.enabled = CamControl.enabled;
            }
            else
            {
                UICanvas.enabled = false;
                Cursor.visible = true;
                CamControl.enabled = false;
                CursorRenderer.enabled = false;

               
            }
            return;
        }
    }


   
    void Update2()
    {
        
        
 

        if (Input.GetButton(modifier))
        {
            if (Input.GetButtonDown(modundo))
                StageStatic.stage.factState.undo();
            else if (Input.GetButtonDown(modredo))
                StageStatic.stage.factState.redo();
            else if (Input.GetButtonDown(modreset))
                StageStatic.stage.factState.softreset();
            else if (Input.GetButtonDown(modsave))
                StageStatic.stage.push_record();
            else if (Input.GetButtonDown(modload))
            {
                StageStatic.stage.factState.hardreset();
                StageStatic.LoadInitStage(StageStatic.stage.name, !StageStatic.stage.use_install_folder);
            }
        }
 
            

        /*
        //Todo before capturing: Make directories "UFrameIT-Screenshots/Unity_ScreenCapture" in project folder
        else if (Input.GetKeyDown(ScreenshotKey)) {
            ScreenCapture.CaptureScreenshot("UFrameIT-Screenshots\\Unity_ScreenCapture\\Capture.png");
        }
        */
    }



    

}
