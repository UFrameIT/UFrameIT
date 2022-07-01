using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using static UIconfig;
using static CommunicationEvents;

public class StartServer : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI WaitingText;

    public static Process process;
    public static ProcessStartInfo processInfo;
    public int autostart = 0; //when 1 in Start() will be ServerRoutine() launched. 
    public int autoend = 0; //Update() also affected
    public int autoprepareGame = 0;
    public int autocheckIfServerIsRunning = 0;


    // Start is called before the first frame update
    void Start()
    {
        CommunicationEvents.ServerRunning = false;
        
        if (ServerAutoStart==true && autostart == 1 )
        {
            StartCoroutine(ServerRoutine());
        }
        if (ServerAutoStart == true && autostart == 2 )
        {
            StartCoroutine(ServerRoutine1());
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (autoend == 1)
        {
            if (CommunicationEvents.ServerRunning && Input.anyKey)
            {
                SceneManager.LoadScene(1);
            }

            //if(!ServerRunning) UnityEngine.Debug.Log("waiting " + ServerRunning);
        }
    }




    void PrepareGame()
    {
        if (autoprepareGame !=0)
        {
            WaitingText.text = "Press any key to start the game";
            CommunicationEvents.ServerRunning = true;
            UnityEngine.Debug.Log("server fin");
        }


    }







    IEnumerator ServerRoutine1()
    {

        string command = "\"" + Application.streamingAssetsPath + "\"/start.BAT " + "\"" + Application.streamingAssetsPath + "\"";
        command = command.Replace("/", @"\");
        command = "\"" + command + "\"";
        UnityEngine.Debug.Log(command);
        ProcessStartInfo processInfo;
        Process process;

        //processInfo = new ProcessStartInfo("cmd.exe", "/C " + command);
        bool cmd = true;
        if (cmd)
        {
            processInfo = new ProcessStartInfo("cmd.exe", "/C " + command);
            //   processInfo.CreateNoWindow = false;
            //  processInfo.UseShellExecute = true;

            process = Process.Start(processInfo);
        }
        else
            /*
            */
            Process.Start("powershell.exe", command);




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
        //  process.Close();






        yield return null;
    }


    IEnumerator ServerRoutine()
    {
        CommunicationEvents.ServerAddressLocal = ServerAddressLocalhost + ":" + ServerPortDefault;
        //print(ServerAdress);
        UnityWebRequest request = UnityWebRequest.Get(CommunicationEvents.ServerAddressLocal + "/scroll/list");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError
         || request.result == UnityWebRequest.Result.ProtocolError)
        {
            UnityEngine.Debug.Log("no running server " + request.error);


#if !UNITY_WEBGL

            //#if UNITY_STANDALONE_LINUX
            //#elif UNITY_STANDALONE_OSX
            //#else
            processInfo = new ProcessStartInfo();
            processInfo.FileName = "java";
            //processInfo.Arguments = @"-jar " + Application.streamingAssetsPath + "/frameit.jar" + " -bind :8085 -archive-root " + Application.streamingAssetsPath + "/archives";
            processInfo.Arguments = @"-jar " + Application.streamingAssetsPath + "/frameit-mmt.jar" + " -bind :" + ServerPortDefault + " -archive-root " + Application.streamingAssetsPath + "/archives";
            //set "UseShellExecute = true" AND "CreateNoWindow = false" to see the mmt-server output
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = !localServerWithAdditionalWindow; // true;
            print("Serverinit: " +  processInfo.Arguments);

            process = Process.Start(processInfo);
            process_mmt_frameIT_server = process;
            
            //#endif
            yield return null;
#endif


            while (true && autocheckIfServerIsRunning != 0)
            {
                //Wait for 2 seconds
                yield return new WaitForSecondsRealtime(2f);
                print("waiting");
                
                request = UnityWebRequest.Get(CommunicationEvents.ServerAddressLocal + "/scroll/list");
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
        }

        PrepareGame();
        yield return null;
    }


}
