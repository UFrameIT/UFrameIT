using UnityEngine;
using static CommunicationEvents;

public class HideUI : MonoBehaviour
{

    public KeyCode Key = KeyCode.F1;
    //public KeyCode ScreenshotKey = KeyCode.F2;

    public string
        modifier,
        modundo,
        modredo,
        modreset;

    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController CamControl;
    public bool LockOnly = true;
    public MeshRenderer CursorRenderer;
    public Canvas UICanvas;

    void Start()
    {
        if (!LockOnly)
        {
            if (UICanvas == null)
                UICanvas = GetComponentInChildren<Canvas>();
            bool camActive = !UICanvas.enabled;
            CamControl.enabled = camActive;
            CursorRenderer.enabled = camActive;
            Debug.Log("camactive = " + camActive.ToString());
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
                //  Rect.localScale = Vector3.one * ((Rect.localScale.x + 1) % 2);
                //bool camActive = Rect.localScale.x != 1;
                bool camActive = UICanvas.enabled;
                UICanvas.enabled = !UICanvas.enabled;
                CamControl.enabled = camActive;
                CursorRenderer.enabled = camActive;
            }
        }

        else if (Input.GetButton(modifier))
        {
            if (Input.GetButtonDown(modundo))
                LevelFacts.undo();
            else if (Input.GetButtonDown(modredo))
                LevelFacts.redo();
            else if (Input.GetButtonDown(modreset))
                LevelFacts.softreset();
        }
        
        /*
        //Todo before capturing: Make directories "UFrameIT-Screenshots/Unity_ScreenCapture" in project folder
        else if (Input.GetKeyDown(ScreenshotKey)) {
            ScreenCapture.CaptureScreenshot("UFrameIT-Screenshots\\Unity_ScreenCapture\\Capture.png");
        }
        */
    }
}
