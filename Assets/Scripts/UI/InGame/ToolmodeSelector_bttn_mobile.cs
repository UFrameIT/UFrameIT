
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static CommunicationEvents;

public class ToolmodeSelector_bttn_mobile : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {


    }

    void Update()
    {

    }


    public void Tool_nextright()
    {    
        CommunicationEvents.ToolID_new= CommunicationEvents.ToolID_selected + 1;
        CommunicationEvents.takeNewToolID = true;
        
    }
    public void Tool_nextleft()
    {
        CommunicationEvents.ToolID_new = CommunicationEvents.ToolID_selected - 1;
        CommunicationEvents.takeNewToolID = true;
    }
}