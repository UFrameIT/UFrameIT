using UnityEngine;
using UnityEngine.UI;

public class GadgetManager : MonoBehaviour
{
    public int FrameITUIversion_ID;
    public GameObject GadgetUI;
    public GameObject GadgetButton;
    public static Gadget activeGadget;
    public static Gadget[] gadgets;

    // Start is called before the first frame update
    void Start()
    {
        if (FrameITUIversion_ID == UIconfig.FrameITUIversion)
        {
            
            Start2();
            
        }
    }
    
    void Start2()
    {
        

        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
        gadgets = GetComponentsInChildren<Gadget>();

        for (int i = 0; i < gadgets.Length; i++)
        {
            gadgets[i].id = i;
            //Create Buttons and add them to UI
            CreateButton(gadgets[i]);

            if (i == 0)
            {
                gadgets[i].gameObject.SetActive(true);
                activeGadget = gadgets[i];
            }
            else
            {
                gadgets[i].gameObject.SetActive(false);
            }
        }

        //Activate UI (using buttons)
        GadgetUI.GetComponent<ToolModeSelector>().enabled = true;
    }

    public void CreateButton(Gadget gadget)
    {


        if (UIconfig.FrameITUIversion == 1)
        {
            CreateButton1(gadget);
        }

        if (UIconfig.FrameITUIversion == 2)
        {
            CreateButton2(gadget);
        }
        if (UIconfig.FrameITUIversion != 2 && UIconfig.FrameITUIversion != 1)
        {
            print("GadgetManager: please select a valid UIconfig.FrameITUIversion, or add how to handle ");
            CreateButton1(gadget);
        }
    }



    public void CreateButton1(Gadget gadget)
    {
        var button = GameObject.Instantiate(GadgetButton);
        button.GetComponent<Image>().sprite = gadget.Sprite;
        button.transform.SetParent(GadgetUI.transform);


        var uiRect = GadgetUI.GetComponent<RectTransform>().rect;
        var buttonRect = button.GetComponent<RectTransform>().rect;

        var completeGadgetsLength = (gadgets.Length * buttonRect.width) + ((gadgets.Length - 1) * buttonRect.width);

        button.transform.localPosition = Vector2.right * (
            -completeGadgetsLength * .5f +//left border of gadgets
             buttonRect.width * .5f + //center of button
             buttonRect.width * 2f * gadget.id); //margin between buttons

    }
    public void CreateButton2(Gadget gadget)
    {
        var button = GameObject.Instantiate(GadgetButton);
        button.GetComponent<Image>().sprite = gadget.Sprite;
        button.transform.SetParent(GadgetUI.transform);


        var uiRect = GadgetUI.GetComponent<RectTransform>().rect;
        var buttonRect = button.GetComponent<RectTransform>().rect;

        var completeGadgetsLength = (gadgets.Length * buttonRect.width) + ((gadgets.Length - 1) * buttonRect.width);

        button.transform.localPosition = Vector2.right * (
                -completeGadgetsLength * 0.5f +//left border of gadgets
                buttonRect.width * .5f + //center of button
                buttonRect.width * 1.2f * gadget.id + //margin between buttons
                completeGadgetsLength * (2f - 1.2f) / 4 //centering because of tightening.
             );
    }

    public void OnToolModeChanged(int id)
    {
        activeGadget.gameObject.SetActive(false);
        activeGadget = gadgets[id];
        activeGadget.gameObject.SetActive(true);
    }

}
