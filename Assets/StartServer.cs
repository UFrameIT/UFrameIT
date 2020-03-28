using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class StartServer : MonoBehaviour
{
   [SerializeField]
    TMPro.TextMeshProUGUI WaitingText;

    bool ServerRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ServerRoutine());
    }
    void PrepareGame()
    {
        WaitingText.text = "Press any key to start the game";
        ServerRunning= true;
        UnityEngine.Debug.Log("server fin");

    }

    IEnumerator ServerRoutine()
    {

        UnityWebRequest request = UnityWebRequest.Get("localhost:8081/scroll/list");
        yield return request.Send();
        if (request.isNetworkError || request.isHttpError)
        {
            UnityEngine.Debug.Log("no running server");

            string command = Application.streamingAssetsPath + "/start.BAT " + Application.streamingAssetsPath;
            UnityEngine.Debug.Log(command);
            int exitCode;
            ProcessStartInfo processInfo;
            Process process;

            processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            // processInfo.RedirectStandardError = true;
            //processInfo.RedirectStandardOutput = true;

            process = Process.Start(processInfo);
            yield return null;

            while (true)
            {
                request = UnityWebRequest.Get("localhost:8081/scroll/list");
                yield return request.Send();
                if (request.isNetworkError || request.isHttpError)
                {
                    UnityEngine.Debug.Log("no running server");
                }
                else
                {
                    break;
                }



                yield return null;
            }

            // *** Read the streams ***
            // Warning: This approach can lead to deadlocks, see Edit #2
            //string output = process.StandardOutput.ReadToEnd();
            //string error = process.StandardError.ReadToEnd();

          //  exitCode = process.ExitCode;
            // UnityEngine.Debug.Log(output);
            // UnityEngine.Debug.Log(error);
            //  Console.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
            // Console.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
            // Console.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
            process.Close();
        }

        PrepareGame();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if(ServerRunning && Input.anyKey)
        {
             SceneManager.LoadScene(1);
        }

        //if(!ServerRunning) UnityEngine.Debug.Log("waiting " + ServerRunning);
    }
}
