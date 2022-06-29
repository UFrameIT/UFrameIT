using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;

using static UIconfig;
using static StreamingAssetLoader;


public class ControlOptionsMenue_mobile : MonoBehaviour
{
    public GameObject TouchControlButtonT;
    public GameObject TouchControlButtonUT;
    public GameObject TouchModeButtonT;
    public GameObject TouchModeButtonUT;
    public GameObject TAV_Slider;
    public GameObject TAvisibilityT;
    public GameObject InputSystemModeButtonT;
    public GameObject InputSystemModeButtonUT;
    public Text InputManager_Txt1a;
    public Text InputManager_Txt1b;


    private Color colChangeable = new Color(1f, 1f, 1f, 0.5f);
    private Color colChangeable2 = new Color(1f, 1f, 1f, 0.5f);

    //public GameObject TouchModeButton;


    //public GameObject back_GObj;

    void Start()
    {
        TAV_Slider.GetComponent<Slider>().value = UIconfig.TAvisibility;
        touchAreaVisibilityBttn();

        UpdateUI_6();
        InputTXTupdate();
    }

    private void Update()
    {
        TAV_Slider.GetComponent<Slider>().value = UIconfig.TAvisibility;
        touchAreaVisibilityBttn();

        UpdateUI_6();
    }

    public void InputTXTupdate()
    {
        UpdateUI_6();
        InputManager_Txt1a.text = "Movement Forward: "  + "\n" +
                                "Movement Backward:  " + "\n" +
                                "Movement Left:      " +  "\n" +
                                "Movement Right:     " + "\n" +
                                "Movement Running:   " + "\n" +
                                "Movement Jump:      " + "\n" +
                                //"\n" +
                                "Action 1:           " + "\n" +
                                "Action 2:           " + "\n" +
                                "Change Tool:        " + "\n" +
                                //"\n" +
                                "Menue Tools:        " + "\n" +
                                "Menue Mathematics:  " + "\n" +
                                "Menue Cancel:       " +  "\n" +
                                //"\n" +
                                "Command Load:       " +  "\n" +
                                "Command Save:       " + "\n" +
                                "Command Reset:       " +  "\n" +
                                "Command Undo:       " + "\n" +
                                "Command Redo:       " +  "\n";
        
        InputManager_Txt1b.text =  InputManager_KeyBinding_Vertical_1 + " , " + InputManager_KeyBinding_Vertical_2 + "\n" +
                        InputManager_KeyBinding_Vertical_01 + " , " + InputManager_KeyBinding_Vertical_02 + "\n" +
                        InputManager_KeyBinding_Horizontal_01 + " , " + InputManager_KeyBinding_Horizontal_02 + "\n" +
                        InputManager_KeyBinding_Horizontal_1 + " , " + InputManager_KeyBinding_Horizontal_2 + "\n" +
                        InputManager_KeyBinding_Running_1 + "\n" +
                         InputManager_KeyBinding_Jump_1 + "\n" +
                        //"\n" +
                        InputManager_KeyBinding_Fire1_1 + "\n" +
                        InputManager_KeyBinding_Fire2_1 + "\n" +
                        InputManager_KeyBinding_MouseScrollWheel_1 + "\n" +
                        //"\n" +
                        InputManager_KeyBinding_ToolmMenue_1 + "\n" +
                         InputManager_KeyBinding_MathMenue_1 + "\n" +
                        InputManager_KeyBinding_Cancel_1 + "\n" +
                        //"\n" +
                         InputManager_KeyBinding_modifier + " + " + InputManager_KeyBinding_mod_load_1 + "\n" +
                        InputManager_KeyBinding_modifier + " + " + InputManager_KeyBinding_mod_save_1 + "\n" +
                         InputManager_KeyBinding_modifier + " + " + InputManager_KeyBinding_mod_reset_1 + "\n" +
                        InputManager_KeyBinding_modifier + " + " + InputManager_KeyBinding_mod_undo_1 + "\n" +
                        InputManager_KeyBinding_modifier + " + " + InputManager_KeyBinding_mod_redo_1 + "\n";



    }


    private void UpdateUI_6()
    {
        switch (UIconfig.controlMode)
        {
            case 1:

                //TouchControlButtonT.GetComponent<Text>().text = "Touch controls: OFF";
                //TouchControlButtonUT.GetComponent<Text>().text = "Press for activating";
                TouchControlButtonT.GetComponent<Text>().text = "Keyboard & Mouse";
                TouchControlButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;

            case 2:

                //TouchControlButtonT.GetComponent<Text>().text = "Touch controls: ON";
                //TouchControlButtonUT.GetComponent<Text>().text = "Press for deactivating";
                TouchControlButtonT.GetComponent<Text>().text = "Touch-Control";
                TouchControlButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;
            default:

                //TouchControlButtonT.GetComponent<Text>().text = "Touch controls: OFF";
                //TouchControlButtonUT.GetComponent<Text>().text = "Press for activating";
                TouchControlButtonT.GetComponent<Text>().text = "Not Defined!";
                TouchControlButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;

        }

        switch (UIconfig.InputManagerVersion)
        {
            case 1:

                InputSystemModeButtonT.GetComponent<Text>().text = "use Input_Manager bindings";
                InputSystemModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;

            case 2:

                InputSystemModeButtonT.GetComponent<Text>().text = "use Input_System_Package";
                InputSystemModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;
            case 3:

                InputSystemModeButtonT.GetComponent<Text>().text = "Input exchange through file";
                InputSystemModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;
            default:

                InputSystemModeButtonT.GetComponent<Text>().text = "Not Defined!";
                InputSystemModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;

        }

        switch (UIconfig.touchControlMode)
        {
            case 1:

                TouchModeButtonT.GetComponent<Text>().text = "Touch mode: BUTTONS";
                TouchModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;

            case 2:


                TouchModeButtonT.GetComponent<Text>().text = "Touch mode: D-PAD";
                TouchModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;
            case 3:


                TouchModeButtonT.GetComponent<Text>().text = "Touch mode: HYBRID";
                TouchModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;
            default:

                TouchModeButtonT.GetComponent<Text>().text = "Not Defined!";
                TouchModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;
        }
        
        //updateUIpreview();
    }

    

    public void TouchControls()
    {
        switch (UIconfig.controlMode)
        {
            case 1:
                UIconfig.controlMode = 2;

                break;

            case 2:
                UIconfig.controlMode = 1;

                break;
            default:
                UIconfig.controlMode = 2;

                break;

        }
        //updateUIpreview();
        NetworkJSON_Save();
    }

    public void TouchControlModes()
    {
        switch (UIconfig.touchControlMode)
        {
            case 1:
                UIconfig.touchControlMode = 2;

                break;

            case 2:
                UIconfig.touchControlMode = 3;

                break;
            case 3:
                UIconfig.touchControlMode = 1;

                break;
            default:
                UIconfig.touchControlMode = 1;

                break;

        }
        //updateUIpreview();
        NetworkJSON_Save();
    }

    public void InputSystemModes()
    {
        switch (UIconfig.InputManagerVersion)
        {
            case 1:
                UIconfig.InputManagerVersion = 2;

                break;

            case 2:
                UIconfig.InputManagerVersion = 3;
                //GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls OFF";
 
                break;
            case 3:
                UIconfig.InputManagerVersion = 1;
                //GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls OFF";

                break;
            default:
                UIconfig.InputManagerVersion = 1;

                break;

        }
        //updateUIpreview();
        NetworkJSON_Save();
    }

    public void touchAreaVisibilityBttn()
    {
        UIconfig.TAvisibility = TAV_Slider.GetComponent<Slider>().value;
        TAvisibilityT.GetComponent<Text>().text = "Touch area visibility " + (int)(100 * UIconfig.TAvisibility) + "%";

        //updateUIpreview();

    }
    

}
