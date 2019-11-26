using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignText : MonoBehaviour
{
    // Start is called before the first frame update

    Camera Cam;

    void Start()
    {
        Cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        transform.forward = Cam.transform.forward;

    }
}
