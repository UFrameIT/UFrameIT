using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignText : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera Cam;

    void Start()
    {
        Cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Cam.transform.forward;
        //Not yet the perfect solution
        //Problem is the relative rotation of the TextMesh to the Line-Parent
        //transform.rotation = Quaternion.Lerp(transform.parent.transform.rotation, Cam.transform.rotation, 0);
    }
}
