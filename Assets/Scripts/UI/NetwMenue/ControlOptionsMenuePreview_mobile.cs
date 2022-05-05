using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;

using static UIconfig;



public class ControlOptionsMenuePreview_mobile : MonoBehaviour
{
    public GameObject ExBttnPreview1;
    public GameObject ExBttnPreview2l;
    public GameObject ExBttnPreview2r;
    public GameObject ExDpadPreview1;
    public GameObject ExDpadPreview2;
    //public GameObject 
    //public GameObject 

    private Color colChangeable = new Color(1f, 1f, 1f, 0.5f);
    private Color colChangeable2 = new Color(1f, 1f, 1f, 0.5f);

    //public GameObject TouchModeButton;


    //public GameObject back_GObj;

    void Start()
    {
        Update();
    }

    private void Update()
    {
        updateUIpreview();
    }


    

    public void updateUIpreview()
    {

        if (UIconfig.controlMode == 2)
        {

            colChangeable.a = UIconfig.TAvisibility;
            colChangeable2.a = (UIconfig.TAvisibility) / 5;
            switch (UIconfig.touchControlMode)
            {
                case 1:
                    ExBttnPreview1.GetComponentInChildren<Image>().color = colChangeable;
                    ExBttnPreview2l.GetComponentInChildren<Image>().color = colChangeable;
                    ExBttnPreview2r.GetComponentInChildren<Image>().color = colChangeable;
                    ExDpadPreview1.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExDpadPreview2.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    break;
                case 2:
                    ExBttnPreview1.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExBttnPreview2l.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExBttnPreview2r.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExDpadPreview1.GetComponentInChildren<Image>().color = colChangeable2;
                    ExDpadPreview2.GetComponentInChildren<Image>().color = colChangeable2;
                    break;
                case 3:
                    ExBttnPreview1.GetComponentInChildren<Image>().color = colChangeable;
                    ExBttnPreview2l.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExBttnPreview2r.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExDpadPreview1.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExDpadPreview2.GetComponentInChildren<Image>().color = colChangeable2;
                    break;



                default:
                    ExBttnPreview1.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExBttnPreview2l.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExBttnPreview2r.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExDpadPreview1.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    ExDpadPreview2.GetComponentInChildren<Image>().color = UIconfig.colClear;
                    break;
            }
        }
        else
        {
            ExBttnPreview1.GetComponentInChildren<Image>().color = UIconfig.colClear;
            ExBttnPreview2l.GetComponentInChildren<Image>().color = UIconfig.colClear;
            ExBttnPreview2r.GetComponentInChildren<Image>().color = UIconfig.colClear;
            ExDpadPreview1.GetComponentInChildren<Image>().color = UIconfig.colClear;
            ExDpadPreview2.GetComponentInChildren<Image>().color = UIconfig.colClear;
        }





    }

 
    

}
