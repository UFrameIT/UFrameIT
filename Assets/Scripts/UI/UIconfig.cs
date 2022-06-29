using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public static class UIconfig
{

    public static int FrameITUIversion = 2; // 1= FrameITUI; 2= FrameITUI_mobil
    public static int InputManagerVersion = 1; // 1= InputManager, 2=InputSystemPackage ; 3=overConfigfile
    public static int GameplayMode = 5; //5=First Person, 4=third, 3=third+, 2=Escaperoom, 1=Sidescroller, 0=Sidescroller+

    public static int MainCameraID = 0; //0=Camera.main; 1=Cam1, 2=Cam2

    public static float[,] DPAD = new float[2, 4] { { 0, 0,0,0 }, { 0, 0, 0, 0 } } ; //Movement, Camera   //Up, Down, Left, Right//

    public static bool localServerWithAdditionalWindow = true;


    //How to handle the waitTime to start ToolModeSelector
    public static int ToolModeSelector_HandlerMode = 1  ; //set 1. At moment no other options available

    public static int GadgetFirstUse = 0;

    public static double cursorSize = 0.03125; //=60/1920;
    public static double cursorSize_SliderMax = 1; //Faktor
    public static double cursorsize_default = 0.03125;
    public static bool checkOperationSystemAlreadyDone = false;

    public static int refHeight = -1;
    public static int refWidth = -1;

    public static int screHeight = -1;
    public static int screWidth = -1;

    public static float scaleMatch = -1;


    public static int Andr_Start_menue_counter = 1;
    //public static int Andr_Start_menue_counter = 1;
    public static int controlMode = 1; //1=keyboard 2=mobile 
    
    

    public static int touchControlMode = 1;
    public static float TAvisibility = 1;
    

    public static Color colOnline = new Color(148f / 255f, 229f / 255f, 156f / 255f, 1f);
    public static Color colOffline = new Color(255f / 255f, 255f / 255f, 255f / 255f, 1f);
    public static Color colPressed = new Color(133f / 255f, 140f / 255f, 107f / 255f, 1f);
    public static Color colSelect = new Color(133f / 255f, 125f / 255f, 107f / 255f, 1f);
    public static Color colClear = new Color(133f / 255f, 125f / 255f, 107f / 255f, 0f);

    public static double colliderScale_all =1;
    public static double colliderScale_all_SliderMax = 5; //Faktor
    public static double colliderScale_all_default = 1;
    public static double colliderScale_PC_default = 1;
    public static double colliderScale_Mobile_default = 4;
    public static double[] colliderScale_Obj_array = new double[10] { 0, 0.5, 0, 0, 0, 0, 0, 0, 0, 0 }; //id 1=tree,

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
        public bool autoOSrecognition;
        public int Opsys;
        public int FrameITUIversion;
        public int InputManagerVersion;
        public double colliderScale_all;
        public double cursorSize;
        
    }

    //INPUTMANAGER KEY BINDINGS LIST for display Purpose
        public static string InputManager_KeyBinding_Horizontal_01 = "left";
        public static string InputManager_KeyBinding_Horizontal_1 = "right";
        public static string InputManager_KeyBinding_Horizontal_02 = "a";
        public static string InputManager_KeyBinding_Horizontal_2= "d";
        public static string InputManager_KeyBinding_Vertical_01 = "down";
        public static string InputManager_KeyBinding_Vertical_1 = "up";
        public static string InputManager_KeyBinding_Vertical_02 = "s";
        public static string InputManager_KeyBinding_Vertical_2 = "w";
        public static string InputManager_KeyBinding_Running_1 = "left shift";
        public static string InputManager_KeyBinding_ToolmMenue_1 = "e";
        public static string InputManager_KeyBinding_MathMenue_1 = "tab";
        public static string InputManager_KeyBinding_Jump_1 = "space";
        public static string InputManager_KeyBinding_Cancel_1 = "escape";
        public static string InputManager_KeyBinding_modifier = "m";
        public static string InputManager_KeyBinding_mod_load_1 = "l";
        public static string InputManager_KeyBinding_mod_save_1 = "s";
        public static string InputManager_KeyBinding_mod_reset_1 = "backspace";
        public static string InputManager_KeyBinding_mod_redo_1 = "r";
        public static string InputManager_KeyBinding_mod_undo_1 = "u";
        public static string InputManager_KeyBinding_Fire1_1 = "Mouse 0";
        public static string InputManager_KeyBinding_Fire2_1 = "Mouse 1";
        public static string InputManager_KeyBinding_MouseScrollWheel_1 = "MouseScrollWheel";


    // 11=TouchUI_onoff
    public static int[] CanvasOnOff_Array = new int[30]; //1= ON 0=off, 2=on_withoutStartdefault , 3=off_withoutStartdefault

    //Which UI to activate after "ContinueGame" from "PauseMenue" 
    public static int[] CanvasOn_afterPM = new int[10] { 10, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public static Boolean GamePaused = false;

    //Time.timeScale for resuming
    public static float Game_TimeScale = 1f;



}