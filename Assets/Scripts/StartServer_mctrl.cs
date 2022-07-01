using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static UIconfig;
using static CommunicationEvents;

public class StartServer_mctrl : MonoBehaviour
{

    public GameObject ServerScriptGObj;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ServerRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }



    IEnumerator ServerRoutine()
    {
        //Wait for 1 seconds
        yield return new WaitForSecondsRealtime(1f);
        if (ServerAutoStart == true)
        {
            ServerScriptGObj.SetActive(true);

        }

    }

}