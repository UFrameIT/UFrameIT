using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUI : MonoBehaviour
{

    public KeyCode Key = KeyCode.F1;
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController CamControl;
    public bool LockOnly = true;
    public MeshRenderer CursorRenderer;

    void Start()
    {
        if (!LockOnly)
        {
            bool camActive = transform.localScale.x != 1;
            CamControl.enabled = camActive;
            CursorRenderer.enabled = camActive;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Key))
        {

            if (LockOnly)
            {
                CamControl.enabled = !CamControl.enabled;
                CursorRenderer.enabled = CamControl.enabled;
            }
            else
            {
                transform.localScale = Vector3.one * ((transform.localScale.x + 1) % 2);
                bool camActive = transform.localScale.x != 1;


                CamControl.enabled = camActive;
                CursorRenderer.enabled = camActive;
            }


        }
    }
}
