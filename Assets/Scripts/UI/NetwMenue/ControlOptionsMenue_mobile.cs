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

    private Color colChangeable = new Color(1f, 1f, 1f, 0.5f);
    private Color colChangeable2 = new Color(1f, 1f, 1f, 0.5f);

    //public GameObject TouchModeButton;


    //public GameObject back_GObj;

    void Start()
    {
        UpdateUI_6();
    }

    private void Update()
    {
        UpdateUI_6();
    }

    void UpdateUI_6()
    {
        switch (UIconfig.controlMode)
        {
            case 1:

                TouchControlButtonT.GetComponent<Text>().text = "Touch controls: OFF";
                TouchControlButtonUT.GetComponent<Text>().text = "Press for activating";
                break;

            case 2:

                TouchControlButtonT.GetComponent<Text>().text = "Touch controls: ON";
                TouchControlButtonUT.GetComponent<Text>().text = "Press for deactivating";
                break;
            default:

                TouchControlButtonT.GetComponent<Text>().text = "Touch controls: OFF";
                TouchControlButtonUT.GetComponent<Text>().text = "Press for activating";
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

                TouchModeButtonT.GetComponent<Text>().text = "Touch mode: D-PAD";
                TouchModeButtonUT.GetComponent<Text>().text = "Press for changing mode";
                break;
        }
        TAV_Slider.GetComponent<Slider>().value = UIconfig.TAvisibility;
        TAvisibilityT.GetComponent<Text>().text = "Touch area visibility " + (int)(100 * UIconfig.TAvisibility) + "%";
        //updateUIpreview();
    }

    

    public void TouchControls()
    {
        switch (UIconfig.controlMode)
        {
            case 1:
                UIconfig.controlMode = 2;
                TouchControlButtonT.GetComponent<Text>().text = "Touch controls: ON";
                TouchControlButtonUT.GetComponentInChildren<Text>().text = "Press for deactivating";
                break;

            case 2:
                UIconfig.controlMode = 1;
                //GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls OFF";
                TouchControlButtonT.GetComponent<Text>().text = "Touch controls: OFF";
                TouchControlButtonUT.GetComponentInChildren<Text>().text = "Press for activating";
                break;
            default:
                UIconfig.controlMode = 2;
                TouchControlButtonT.GetComponent<Text>().text = "Touch controls: ON";
                TouchControlButtonUT.GetComponentInChildren<Text>().text = "Press for deactivating";
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
                TouchModeButtonT.GetComponent<Text>().text = "Touch mode: D-PAD";
                TouchModeButtonUT.GetComponentInChildren<Text>().text = "Press for changing mode";
                break;

            case 2:
                UIconfig.touchControlMode = 3;
                //GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls OFF";
                TouchModeButtonT.GetComponent<Text>().text = "Touch mode: HYBRID";
                TouchModeButtonUT.GetComponentInChildren<Text>().text = "Press for changing mode";
                break;
            case 3:
                UIconfig.touchControlMode = 1;
                //GameObject.Find("TextSlotTOO").GetComponent<Text>().text = "Touch controls OFF";
                TouchModeButtonT.GetComponent<Text>().text = "Touch mode: BUTTONS";
                TouchModeButtonUT.GetComponentInChildren<Text>().text = "Press for changing mode";
                break;
            default:
                UIconfig.touchControlMode = 1;
                TouchModeButtonT.GetComponent<Text>().text = "Touch mode: BUTTONS";
                TouchModeButtonUT.GetComponentInChildren<Text>().text = "Press for changing mode";
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
