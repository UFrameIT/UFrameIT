using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static CommunicationEvents;


public class CheckServer : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI WaitingText;

    public static Process process;
    public static ProcessStartInfo processInfo;
    


    // Start is called before the first frame update
    void Start()
    {
        //CommunicationEvents.ServerRunning = false;
        //StartCoroutine(ServerRoutine());

        StartCoroutine(waiter(CommunicationEvents.lastIP, 1, CommunicationEvents.IPcheckGeneration));
        StartCoroutine(waiter(CommunicationEvents.newIP, 2, CommunicationEvents.IPcheckGeneration));
        StartCoroutine(waiter(CommunicationEvents.IPslot1, 3, CommunicationEvents.IPcheckGeneration));
        StartCoroutine(waiter(CommunicationEvents.IPslot2, 4, CommunicationEvents.IPcheckGeneration));
        StartCoroutine(waiter(CommunicationEvents.IPslot3, 5, CommunicationEvents.IPcheckGeneration));
        StartCoroutine(waiter(CommunicationEvents.IPslot3, 6, CommunicationEvents.IPcheckGeneration));


    }

    public void CheckIPAdr()
    {
        //CommunicationEvents.ServerRunning = false;
        //StartCoroutine(ServerRoutine());

        //CommunicationEvents.IPcheckGeneration++;
        //StartCoroutine(waiter(CommunicationEvents.lastIP, 1, CommunicationEvents.IPcheckGeneration));
        //StartCoroutine(waiter(CommunicationEvents.newIP, 2, CommunicationEvents.IPcheckGeneration));
        //StartCoroutine(waiter(CommunicationEvents.IPslot1, 3, CommunicationEvents.IPcheckGeneration));
        //StartCoroutine(waiter(CommunicationEvents.IPslot2, 4, CommunicationEvents.IPcheckGeneration));
        //StartCoroutine(waiter(CommunicationEvents.IPslot3, 5, CommunicationEvents.IPcheckGeneration));


    }


    IEnumerator waiter(String NetwAddress, int NA_id, double ics)
    {
        


        //while(CommunicationEvents.IPcheckGeneration== ics)
        while (CheckNetLoop == 1)
        {
            //Wait for 1 seconds
            yield return new WaitForSecondsRealtime(1f);

            if(CommunicationEvents.CheckServerA[NA_id] == 1)
            {
                CommunicationEvents.CheckServerA[NA_id] = 0;

                    switch (NA_id){
                        case 1:
                            NetwAddress = CommunicationEvents.lastIP;
                            break;
                        case 2:
                            NetwAddress = CommunicationEvents.newIP;
                            break;
                        case 3:
                            NetwAddress = CommunicationEvents.IPslot1;
                            break;
                        case 4:
                            NetwAddress = CommunicationEvents.IPslot2;
                            break;
                        case 5:
                            NetwAddress = CommunicationEvents.IPslot3;
                            break;
                        case 6:
                            NetwAddress = CommunicationEvents.selecIP;
                            break;
                            //default:


                    }

                if (string.IsNullOrEmpty(NetwAddress))
                {
                    //Wait for 1 seconds
                    CommunicationEvents.ServerRunningA[NA_id] = 3;
                    yield return new WaitForSecondsRealtime(1f);
                }
                else
                {
                    StartCheck(NetwAddress, NA_id, ics);
                    //Wait for 0,5 seconds
                    yield return new WaitForSecondsRealtime(0.5f);
                    if (CommunicationEvents.IPcheckGeneration <= ics || (NA_id != 6))// && NA_id != 2))
                    {
                        //if (CommunicationEvents.IPcheckGeneration < ics) { break; }
                        if (CommunicationEvents.ServerRunningA_test[NA_id] == true)
                        {
                            CommunicationEvents.ServerRunningA[NA_id] = 2;
                        }
                        else
                        {
                            CommunicationEvents.ServerRunningA[NA_id] = 0;
                        }
                        //Wait for 0,5 seconds
                        yield return new WaitForSecondsRealtime(0.5f);
                    }
                    else
                    {

                        CommunicationEvents.IPcheckGeneration--;

                        if (NA_id == 2)
                        {
                            CommunicationEvents.ServerRunningA[NA_id] = 1;
                            //Wait for 0,5 seconds
                            yield return new WaitForSecondsRealtime(0.5f);
                        }

                    }
                }
            }


        }
    }

        public void StartCheck(String NetwAddress, int NA_id, double ics)
    {
        CommunicationEvents.ServerRunningA_test[NA_id] = false;
        StartCoroutine(ServerRoutine(NetwAddress, NA_id, ics));
    }

    void PrepareGame()
    {
        //WaitingText.text = "Press any key to start the game";
        //CommunicationEvents.ServerRunning_test = true;
        //UnityEngine.Debug.Log("server fin");

    }

    IEnumerator ServerRoutine(String NetwAddress, int NA_id, double ics)
    {
        UnityWebRequest request = UnityWebRequest.Get("http://" + NetwAddress + "/scroll/list");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError
         || request.result == UnityWebRequest.Result.ProtocolError)
        {
            UnityEngine.Debug.Log("no running server " + request.error);


            while (true)
            {
                request = UnityWebRequest.Get("http://" + NetwAddress + "/scroll/list");
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.ConnectionError
                 || request.result == UnityWebRequest.Result.ProtocolError)
                {
                    // UnityEngine.Debug.Log("no running server");
                }
                else
                {
                    break;
                }



                yield return null;
            }
        }

        //PrepareGame(); 
        if (CommunicationEvents.IPcheckGeneration == ics || (NA_id!=6))// && NA_id!=2))
        {
            CommunicationEvents.ServerRunningA_test[NA_id] = true;
        }
        yield return null;
    }

    


    void Update()
    {
        
    }

}
