using UnityEngine;
using UnityEngine.SceneManagement;
using static CommunicationEvents;
using static UIconfig;
//using static CamControl_1;

public class HideUI : MonoBehaviour
{

    //public KeyCode Key = KeyCode.F1;
    //public KeyCode ScreenshotKey = KeyCode.F12;

    public string
        modifier,
        modundo,
        modredo,
        modreset,
        modsave,
        modload;
    public string cancel_keyBind;
    public string MathM_keyBind;



    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController CamControl_StdAsset;
    public Characters.FirstPerson.FirstPersonController1 CamControl_ScriptChar;


    public bool LockOnly = true;
    public MeshRenderer CursorRenderer;
    //for Debug:
    //public MeshRenderer CursorRenderer_FirstP; 
    //public MeshRenderer CursorRenderer_FirstP_oldInpOrig;
    //public int whichCursor;



    internal Canvas UICanvas;

    void Start()
    {
        if (UIconfig.FrameITUIversion == 1) // 1= FrameITUI; 2= FrameITUI_mobil
        {
            Start2();
        }
        else
        {
            if (UICanvas == null)
            {
                UICanvas = GetComponentInChildren<Canvas>();
            }
            UICanvas.enabled = false;
        }

    }
    void Start2()
    {

        if (!LockOnly)
        {
            if (UICanvas == null)
            {
                UICanvas = GetComponentInChildren<Canvas>();
            }
            bool camActive;
            camActive = !UICanvas.enabled;
            //camActive = false;
            SetCamControl123(camActive);
            SetCursorRenderer123(camActive);
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (UIconfig.FrameITUIversion == 1)
        {
            Update2();
        }
    }

    void Update2() { 

        if (Input.GetButtonDown(MathM_keyBind))
        {
            if (LockOnly)
            {
                CamControl_StdAsset.enabled = !CamControl_StdAsset.enabled;
                SetCursorRenderer123(CamControl_StdAsset.enabled);
                SetCamControl123(CamControl_StdAsset.enabled);
                
            }
            else
            {
                Cursor.visible = !UICanvas.enabled;
                SetCamControl123(UICanvas.enabled);

                SetCursorRenderer123(UICanvas.enabled);

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
            else if (Input.GetButtonDown(modload))
            {
                StageStatic.stage.factState.hardreset();
                StageStatic.LoadInitStage(StageStatic.stage.name, !StageStatic.stage.use_install_folder);
            }
        }
        if (Input.GetButtonDown(cancel_keyBind))
        {

            UIconfig.CanvasOnOff_Array[02] = 1;
            //UIconfig.CanvasOnOff_Array[10] = 0;
            return;
        }

        /*
        //Todo before capturing: Make directories "UFrameIT-Screenshots/Unity_ScreenCapture" in project folder
        else if (Input.GetKeyDown(ScreenshotKey)) {
            ScreenCapture.CaptureScreenshot("UFrameIT-Screenshots\\Unity_ScreenCapture\\Capture.png");
        }
        */
    }

    private void SetCursorRenderer123(bool opt)
    {
        CursorRenderer.enabled = opt;

        //multiple Cursor result in conflicts
        /*
        switch (whichCursor)
        //switch (UIconfig.GameplayMode)
        {
            case 0:
                CursorRenderer_FirstP_oldInpOrig.enabled = opt;
                break;

            case 5:
                CursorRenderer_FirstP.enabled = opt;
                break;
            case 6:
                CursorRenderer_FirstP_oldInpOrig.enabled = opt;
                break;
            default:
                CursorRenderer_FirstP.enabled = opt;
                break;
                
        }
        */
    }
    private void SetCamControl123(bool opt)
    {
        CamControl_StdAsset.enabled = opt;
        CamControl_ScriptChar.enabled = opt;
    }
}
