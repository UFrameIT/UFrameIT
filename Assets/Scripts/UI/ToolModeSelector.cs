using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UIconfig;

public class ToolModeSelector : MonoBehaviour
{
    private Button[] Buttons;
    private HideUI UIManager;
    private HideUI_mobile UIManager2;
    private Canvas ParentCanvas;
    private Canvas UIManager_Canvas;
    private bool Showing = true;
    private float activeGadgetScaleFactor = 1.5f;
    private bool initUpdate = false;

    // Start is called before the first frame update
    void Start()
    {
        if (UIconfig.ToolModeSelector_HandlerMode == 1)
        {
            if (UIconfig.FrameITUIversion == 1)
            {
                activeGadgetScaleFactor = 1.5f;
            }
            if (UIconfig.FrameITUIversion == 2)
            { 
                activeGadgetScaleFactor = 2.1f;

            }
            myStart();
        }
        if (UIconfig.ToolModeSelector_HandlerMode == 2)
        {
            
            initUpdate = false;
            myStart();
        }
       




    }

    public void myStart()
    {
        //Requires buttons to be in the same order as the toolmode enum
        //We could fully generate the buttons instead if we have icons with file names matching the enums
        var buttons = GetComponentsInChildren<Button>();
        Buttons = buttons.OrderBy(x => x.transform.position.x).ToArray();
        ParentCanvas = GetComponentInParent<Canvas>();
        for (int i = 0; i < Buttons.Length; ++i)
        {
            int copiedIndex = i; //this is important
            var button = Buttons[i];
            button.onClick.AddListener(() => Select(copiedIndex));

        }


        Buttons[GadgetManager.activeGadget.id].transform.localScale *= activeGadgetScaleFactor;

        if (UIconfig.FrameITUIversion == 1)
        {
            UIManager = GetComponentInParent<HideUI>();
            //UIManager_Canvas = GetComponentInParent<HideUI>().UICanvas;
            activeGadgetScaleFactor = 1.5f;
        }
        if (UIconfig.FrameITUIversion == 2)
        {
            UIManager_Canvas = GetComponentInParent<HideUI_mobile>().UICanvas;
            UIManager2 = GetComponentInParent<HideUI_mobile>();
            activeGadgetScaleFactor = 2.1f;

        }

        initUpdate = true;
        

    }

    public void Select(int id)
    {

        ParentCanvas.enabled = true;

        Buttons[GadgetManager.activeGadget.id].transform.localScale /= activeGadgetScaleFactor;
        CommunicationEvents.ToolModeChangedEvent.Invoke(id);
        Buttons[GadgetManager.activeGadget.id].transform.localScale *= activeGadgetScaleFactor;
        StartCoroutine(HideRoutine());

    }

    IEnumerator HideRoutine()
    {

        yield return new WaitForSeconds(2);
        if (!Showing)
        {
           
            ParentCanvas.enabled = false;
        }

    }


    // Update is called once per frame
    void Update()
    {
        //Used for reactivating of the Hitboxes for the pointer Gadget        
        if(GadgetFirstUse == 0)
        {
            int id1 = 1;
            Select(id1);
            GadgetFirstUse++;
        }
        if (GadgetFirstUse == 1)
        {
            int id0 = 0;
            Select(id0);
            GadgetFirstUse++;

        }

        if (initUpdate == true) {
            Update2();
        }
    }

    void Update2()
    {
        
        if (CommunicationEvents.takeNewToolID)
        {

            CheckToolModeSelection_set1();

        }
        else
        {
            
            if (UIconfig.FrameITUIversion == 1)
            {

                //Check if the ToolMode was switched
                if (!UIManager.UICanvas.enabled)
                //if (!UIManager_Canvas.enabled)
                {
                  
                    CheckToolModeSelection();
                }
            }
            if (UIconfig.FrameITUIversion == 2)
            {
                //Check if the ToolMode was switched
                if (!UIManager_Canvas.enabled)
                //if (!UIManager2.enabled) //funktioniert nicht
                { 
                    CheckToolModeSelection();
                }
            }
        }
        CommunicationEvents.ToolID_selected = GadgetManager.activeGadget.id;
    }

    //Checks if the ToolMode was switched by User, and handle it
    void CheckToolModeSelection()
    {
        if (Input.GetButtonDown("ToolMode"))
        {
            Gadget tempActiveGadget = GadgetManager.activeGadget;
            int id = (tempActiveGadget.id + 1) % GadgetManager.gadgets.Length;
            Select(id);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {

            Gadget tempActiveGadget = GadgetManager.activeGadget;
            int move = (int)Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
            move = tempActiveGadget.id + move;
            CheckToolModeSelection_subF1(move);
        }

    }

    void CheckToolModeSelection_set1()
    {
        int move = (int)CommunicationEvents.ToolID_new;
        CheckToolModeSelection_subF1(move);
        CommunicationEvents.takeNewToolID = false;

    }

    void CheckToolModeSelection_subF1(int move)
    {

        int id = (move) % Buttons.Length;// GadgetManager.gadgets.Length;
        if (id < 0) id = Buttons.Length - 1;// GadgetManager.gadgets.Length-1;
        Select(id);
        //print("subF1 select " + id);

    }
}
