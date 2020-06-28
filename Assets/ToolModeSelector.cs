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

    // Start is called before the first frame update
    void Start()
    {
        //Requires buttons to be in the same order as the toolmode enum
        //We could fully generate the buttons instead if we have icons with file names matching the enums
        var buttons = GetComponentsInChildren<Button>();
        Buttons = buttons.OrderBy(x => x.transform.position.x).ToArray();

        for(int i = 0; i< Buttons.Length;++i)
        {
            int copiedIndex = i; //this is important
            var button = Buttons[i];
            button.onClick.AddListener(()=> Select(copiedIndex));
            
        }

    
        Buttons[(int)CommunicationEvents.ActiveToolMode].transform.localScale *= 2;
        UIManager = GetComponentInParent<HideUI>();

    }

    public void Select(int id)
    {

        Buttons[(int)CommunicationEvents.ActiveToolMode].transform.localScale /= 2;
        CommunicationEvents.ToolModeChangedEvent.Invoke((ToolMode)id);
        Buttons[(int)CommunicationEvents.ActiveToolMode].transform.localScale *= 2;
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
            ToolMode tempActiveToolMode = CommunicationEvents.ActiveToolMode;
            int id = ((int)tempActiveToolMode + 1) % System.Enum.GetNames(typeof(ToolMode)).Length;
           // tempActiveToolMode =(ToolMode) id ;
            //Invoke the Handler for the Facts
            // CommunicationEvents.ToolModeChangedEvent.Invoke(tempActiveToolMode);
            Select(id);
        }else if(Input.GetAxis("Mouse ScrollWheel") !=0){

            int move = (int) Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));

            ToolMode tempActiveToolMode = CommunicationEvents.ActiveToolMode;
            int id = ((int)tempActiveToolMode + move) % Buttons.Length;// System.Enum.GetNames(typeof(ToolMode)).Length;
            if (id < 0) id = Buttons.Length - 1;// System.Enum.GetNames(typeof(ToolMode)).Length-1;
            Select(id);
        }


    }
}
