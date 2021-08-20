using UnityEngine;

public class ToolModeText : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
    }

    void OnToolModeChanged(int id)
    {

        //When ToolMode changes: Change Text of active gadget
        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = GadgetManager.gadgets[id].UiName;
    }
}
