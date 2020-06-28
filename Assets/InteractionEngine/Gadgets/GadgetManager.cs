using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgetManager : MonoBehaviour
{

    Dictionary<ToolMode,Gadget> modeToGadget = new Dictionary<ToolMode, Gadget>();
    public GameObject GadgetUI;
    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        var gadgets = GetComponentsInChildren<Gadget>();

        Debug.Log(gadgets.Length);
        foreach (var gadget in gadgets)
        {
       
            modeToGadget.Add(gadget.ToolMode, gadget);
            CreateButton(gadget);
            gadget.gameObject.SetActive(false);
        }
        modeToGadget[CommunicationEvents.ActiveToolMode].gameObject.SetActive(true);
        GadgetUI.GetComponent<ToolModeSelector>().enabled = true;
    }

    public void CreateButton(Gadget gadget)
    {
        var button = GameObject.Instantiate(Resources.Load("Prefabs/GadgetButton") as GameObject);
        button.GetComponent<Image>().sprite = gadget.Sprite;
        button.transform.SetParent(GadgetUI.transform);
        var uiRect = GadgetUI.GetComponent<RectTransform>().rect;
        var buttonRect = button.GetComponent<RectTransform>().rect;
        button.transform.localPosition = Vector2.right*(-.5f * uiRect.width //left border
            + buttonRect.width * .75f //border distance including button width
            + buttonRect.width * 1.5f * (int)gadget.ToolMode); //offset
           
    }

    public void OnToolModeChanged(ToolMode ActiveToolMode)
    {
        modeToGadget[CommunicationEvents.ActiveToolMode].gameObject.SetActive(false);
        CommunicationEvents.ActiveToolMode = ActiveToolMode;
        modeToGadget[CommunicationEvents.ActiveToolMode].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
