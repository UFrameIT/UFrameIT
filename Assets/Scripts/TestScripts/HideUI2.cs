using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUI2 : MonoBehaviour
{

    public KeyCode Key = KeyCode.F1;
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController CamControl;
    public bool manuActive = true;
    public MeshRenderer CursorRenderer;
    

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            manuActive = !manuActive;
            if (manuActive)
            {
                CamControl.enabled = false;
                CursorRenderer.enabled = false;
                
            }
            else
            {
                CamControl.enabled = true;
                CursorRenderer.enabled = true;
            }


        }
    }
}
