using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UIconfig;

public class AlignText : MonoBehaviour
{
    // Start is called before the first frame update

    private Camera Cam;
    public Camera Cam1;
    public Camera Cam2;
    public Camera BackUPCam;

    void Start()
    {
        Cam = toCamMain();
        
        StartCoroutine(CheckForNewMainCamRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = Cam.transform.forward;
        //Not yet the perfect solution
        //Problem is the relative rotation of the TextMesh to the Line-Parent
        //transform.rotation = Quaternion.Lerp(transform.parent.transform.rotation, Cam.transform.rotation, 0);
    }

    Camera toCamMain()
    {
        if (Camera.main != null)
        {
            return Camera.main;
        }


        return BackUPCam;
    }



    IEnumerator CheckForNewMainCamRoutine()
    {

        yield return new WaitForSeconds(2);
        switch (UIconfig.MainCameraID)
        {
                        
            
            case 0:
                Cam = toCamMain();
                break;
            case 1:
                Cam = Cam1;
                break;
            case 2:
                Cam = Cam2;
                break;
            default:
                Cam = toCamMain();
                break;

        }
        //StopCoroutine(CheckForNewMainCamRoutine());
        //print("Stopped:CheckForNewMainCamRoutine()");
        //Cam = Camera.main;
    }


}
