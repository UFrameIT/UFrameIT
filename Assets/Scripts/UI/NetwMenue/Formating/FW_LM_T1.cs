using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //andr
using UnityEngine.SceneManagement;
using System.IO; //
using UnityEngine.Video;//streaming
using UnityEngine.Networking;
using static StreamingAssetLoader;
using static CheckServer;
using static CommunicationEvents;


public class FW_LM_T1 : MonoBehaviour
{
    void Start()
    {
        //POSITIONEN
        Vector3 gsfg = new Vector3(110f, 110f, 10f);
        //GameObject.Find("FrameWorld").transform.localPosition = gsfg;

        //GameObject.Find("FrameWorld_LM_T1").GetComponent<Image>().color = Color.black;
        //GameObject.Find("FrameWorld_LM_T1").transform.localPosition = gsfg;
        //GameObject.Find("FrameWorld_LM_T1").GetComponent<RectTransform>().position = gsfg;
        //GameObject.Find("FrameWorld_LM_T1").GetComponent<RectTransform>().localScale = gsfg;

    }
    void update()
    {

    }
}



