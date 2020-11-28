using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GadgetManager : MonoBehaviour
{
    public GameObject GadgetUI;
    public static Gadget activeGadget;
    public static Gadget[] gadgets;

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        gadgets = GetComponentsInChildren<Gadget>();

        Debug.Log(gadgets.Length);
        for (int i = 0; i < gadgets.Length; i++) {
            gadgets[i].id = i;
            //Create Buttons and add them to UI
            CreateButton(gadgets[i]);

            if (i == 0)
            {
                gadgets[i].gameObject.SetActive(true);
                activeGadget = gadgets[i];
            }
            else {
                gadgets[i].gameObject.SetActive(false);
            }
        }

        //Activate UI (using buttons)
        GadgetUI.GetComponent<ToolModeSelector>().enabled = true;
    }

    public void CreateButton(Gadget gadget)
    {
        var button = GameObject.Instantiate(Resources.Load("Prefabs/GadgetButton") as GameObject);
        button.GetComponent<Image>().sprite = gadget.Sprite;
        button.transform.SetParent(GadgetUI.transform);
        var uiRect = GadgetUI.GetComponent<RectTransform>().rect;
        var buttonRect = button.GetComponent<RectTransform>().rect;
        button.transform.localPosition = Vector2.right*(
            - uiRect.width *.5f +//left border
             buttonRect.width * .75f+ //border distance including button width
             buttonRect.width * 1f * gadget.id); //offset
           
    }

    public void OnToolModeChanged(int id)
    {
        activeGadget.gameObject.SetActive(false);
        activeGadget = gadgets[id];
        activeGadget.gameObject.SetActive(true);
    }

}
