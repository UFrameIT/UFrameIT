using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ToolModeSelector : MonoBehaviour
{
    private Button[] Buttons;
    private HideUI UIManager;
    private Canvas ParentCanvas;
    private bool Showing = false;

    // Start is called before the first frame update
    void Start()
    {
        //Requires buttons to be in the same order as the toolmode enum
        //We could fully generate the buttons instead if we have icons with file names matching the enums
        var buttons = GetComponentsInChildren<Button>();
        Buttons = buttons.OrderBy(x => x.transform.position.x).ToArray();
        ParentCanvas = GetComponentInParent<Canvas>();
        for(int i = 0; i< Buttons.Length;++i)
        {
            int copiedIndex = i; //this is important
            var button = Buttons[i];
            button.onClick.AddListener(()=> Select(copiedIndex));
            
        }

    
        Buttons[GadgetManager.activeGadget.id].transform.localScale *= 2;
        UIManager = GetComponentInParent<HideUI>();

    }

    public void Select(int id)
    {

        ParentCanvas.enabled = true;
        Showing = true;
        
        Buttons[GadgetManager.activeGadget.id].transform.localScale /= 2;
        CommunicationEvents.ToolModeChangedEvent.Invoke(id);
        Buttons[GadgetManager.activeGadget.id].transform.localScale *= 2;
        StartCoroutine(HideRoutine());

    }

    IEnumerator HideRoutine()
    {
        
        yield return new WaitForSeconds(2);
        //if (!Showing)
        {
            ParentCanvas.enabled = false;
        }

    }


    // Update is called once per frame
    void Update()
    {   //Check if the ToolMode was switched
        if(!UIManager.UICanvas.enabled)
            CheckToolModeSelection();
        
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
        else if(Input.GetAxis("Mouse ScrollWheel") !=0){

            int move = (int) Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));

            Gadget tempActiveGadget = GadgetManager.activeGadget;
            int id = (tempActiveGadget.id + move) % Buttons.Length;// GadgetManager.gadgets.Length;
            if (id < 0) id = Buttons.Length - 1;// GadgetManager.gadgets.Length-1;
            Select(id);
        }
        
    }
}
