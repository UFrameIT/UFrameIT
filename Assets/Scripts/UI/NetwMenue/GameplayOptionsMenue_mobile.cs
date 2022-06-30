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


public class GameplayOptionsMenue_mobile : MonoBehaviour
{

    public GameObject cllscaleAll_Slider;
    public GameObject cllscaleAll_SliderT;
    public GameObject cursorScaler_Slider;
    public GameObject cursorScaler_SliderT;
    public GameObject CamSens_Slider;
    public GameObject CamSens_SliderT;

    public Texture2D cursorArrow_35;
    public Texture2D cursorArrow_50;
    public Texture2D cursorArrow_60;
    public Texture2D cursorArrow_70;
    public Texture2D cursorArrow_100;
    public Texture2D cursorArrow_140;
    public Texture2D cursorArrow_200;
    public Texture2D cursorArrow_300;



    private Color colChangeable = new Color(1f, 1f, 1f, 0.5f);
    private Color colChangeable2 = new Color(1f, 1f, 1f, 0.5f);



    void Start()
    {
        
    }

    private void Update()
    {
        cllscaleAll_Slider.GetComponent<Slider>().value = (float)(UIconfig.colliderScale_all / (UIconfig.colliderScale_all_default * UIconfig.colliderScale_all_SliderMax));
        ScaleColliderAllBttn();

        cursorScaler_Slider.GetComponent<Slider>().value = (float)((UIconfig.cursorSize) / (UIconfig.cursorsize_default * UIconfig.cursorSize_SliderMax));
        mousePointerScaleBttn();

        CamSens_Slider.GetComponent<Slider>().value = (float)((UIconfig.camRotatingSensitivity) / (UIconfig.camRotatingSensitivity_default * UIconfig.camRotatingSensitivity_sliderMax));
        CamSensitivityBttn();

    }



    



    public void ScaleColliderAllBttn()
    {
        UIconfig.colliderScale_all = cllscaleAll_Slider.GetComponent<Slider>().value* UIconfig.colliderScale_all_SliderMax* UIconfig.colliderScale_all_default;
        cllscaleAll_SliderT.GetComponent<Text>().text = "Scale of Hitbox for MouseClicks is " + (int)(100 * UIconfig.colliderScale_all/UIconfig.colliderScale_all_default) + "%";

        //updateUIpreview();

    }
    
    public void mousePointerScaleBttn()
    {
        UIconfig.cursorSize = cursorScaler_Slider.GetComponent<Slider>().value * UIconfig.cursorSize_SliderMax *UIconfig.cursorsize_default;
        double zwischenRechn = 100 * (UIconfig.cursorSize)/ (UIconfig.cursorsize_default);
        cursorScaler_SliderT.GetComponent<Text>().text = "Size of MouseCursor is " + (int)(zwischenRechn) + "%";
        setMouse();
    }
    public void CamSensitivityBttn()
    {
        UIconfig.camRotatingSensitivity = CamSens_Slider.GetComponent<Slider>().value * UIconfig.camRotatingSensitivity_sliderMax * UIconfig.camRotatingSensitivity_default;
        double zwischenRechn = 100 * (UIconfig.camRotatingSensitivity); // /(UIconfig.camRotatingSensitivity_default);
        CamSens_SliderT.GetComponent<Text>().text = "Sensitivity of Camera is " + (int)(zwischenRechn) + "%";
        
    }

    public void setMouse()
    {

        print("OPSYS: " + CommunicationEvents.Opsys);
        if (CommunicationEvents.Opsys == 1)
        {
            CommunicationEvents.CursorVisDefault = false;
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
            CommunicationEvents.CursorVisDefault = true;
        }

        //Android crashes in level scene;
        if (CommunicationEvents.Opsys != 1)
        {
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


}
