using UnityEngine;
using UnityEngine.SceneManagement;
using static CommunicationEvents;


//War ein Auslagerungsversuch bzgl HideUI

public class CamControl_1: MonoBehaviour
{

    
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController CamControl_StdAsset;
    public Characters.FirstPerson.FirstPersonController1 CamControl_ScriptChar;
    


    void Start()
    {
     

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void setCamControlEnabled(bool opt)
    {
        CamControl_StdAsset.enabled = opt;
        CamControl_ScriptChar.enabled = opt;
        //Todo
    }


}