using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using static UIconfig;

public class WaitingForLocalMMT : MonoBehaviour
{
   
    

    private void Start()
    {
        //StartCoroutine(ServerRoutine());

    }

    private void Update()
    {
        

      

    }
    private void OnEnable()
    {
        StartCoroutine(ServerRoutine());
    }

    private void OnDisable()
    {
        StopCoroutine(ServerRoutine());
    }

    void PrepareGame()
    {
        if (true)
        {
            
            CommunicationEvents.ServerRunning = true;
            UnityEngine.Debug.Log("set server runs");
        }
        if (CommunicationEvents.ServerRunning == true)
        {
            CommunicationEvents.ServerAdress = CommunicationEvents.ServerAddressLocal;
            UnityEngine.Debug.Log("StartMainMenue");
            SceneManager.LoadScene("MainMenue");
        }


    }

    IEnumerator ServerRoutine()
    {
        while (true)
        {
                

            UnityWebRequest request = UnityWebRequest.Get(CommunicationEvents.ServerAddressLocal + "/scroll/list");
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError
                 || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    // UnityEngine.Debug.Log("no running server");
                }
                else
                {
                    //break;
                    PrepareGame();
                }



                

                //Wait for 2 seconds
                yield return new WaitForSecondsRealtime(2f);
            print("waiting");

            yield return null;
        }


    }
}