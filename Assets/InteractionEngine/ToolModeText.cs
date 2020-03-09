using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolModeText : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ToolModeChangedEvent.AddListener(OnToolModeChanged);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnToolModeChanged(ToolMode ActiveToolMode) {

        //When ToolMode changes: Change Text of active ToolMode
        switch (ActiveToolMode) {
            case ToolMode.MarkPointMode:
                gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Point Mode";
                break;
            case ToolMode.CreateLineMode:
                gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Distance Mode";
                break;
            case ToolMode.CreateAngleMode:
                gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Angle Mode";
                break;
            case ToolMode.CreateRayMode:
                gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Straight Mode";
                break;
            case ToolMode.ExtraMode:
                gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = "Extra Mode";
                break;
        }
    }
}
