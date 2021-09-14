using UnityEngine;
using UnityEngine.SceneManagement;
using static CommunicationEvents;

public class HideUI : MonoBehaviour
{

    public KeyCode Key = KeyCode.F1;
    //public KeyCode ScreenshotKey = KeyCode.F2;

    public string
        modifier,
        modundo,
        modredo,
        modreset,
        modsave,
        modload;

    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController CamControl;
    public bool LockOnly = true;
    public MeshRenderer CursorRenderer;
    internal Canvas UICanvas;

    void Start()
    {
        if (!LockOnly)
        {
            if (UICanvas == null)
                UICanvas = GetComponentInChildren<Canvas>();
            bool camActive = !UICanvas.enabled;
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
                CamControl.enabled = UICanvas.enabled;
                CursorRenderer.enabled = UICanvas.enabled;

                UICanvas.enabled = !UICanvas.enabled;
            }
        }

        else if (Input.GetButton(modifier))
        {
            if (Input.GetButtonDown(modundo))
                StageStatic.stage.factState.undo();
            else if (Input.GetButtonDown(modredo))
                StageStatic.stage.factState.redo();
            else if (Input.GetButtonDown(modreset))
                StageStatic.stage.factState.softreset();
            else if (Input.GetButtonDown(modsave))
                StageStatic.stage.push_record();
            else if (Input.GetButtonDown(modload)) {
                StageStatic.stage.factState.hardreset();
                StageStatic.LoadInitStage(StageStatic.stage.name, !StageStatic.stage.use_install_folder);
            }
        }
        
        /*
        //Todo before capturing: Make directories "UFrameIT-Screenshots/Unity_ScreenCapture" in project folder
        else if (Input.GetKeyDown(ScreenshotKey)) {
            ScreenCapture.CaptureScreenshot("UFrameIT-Screenshots\\Unity_ScreenCapture\\Capture.png");
        }
        */
    }
}
