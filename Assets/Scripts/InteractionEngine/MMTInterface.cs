using System.Diagnostics;
using UnityEngine;

public class MMTInterface : MonoBehaviour
{
    private Process mmtServerProcess;
    private ProcessStartInfo mmtServerProcessInfo;

    // Start is called before the first frame update


    void Start()
    {
        //Start the mmt.bat for carrying out http-Requests
        //TODO: Putting the absolute path to mmt.bat here should soon be changed
        /*mmtServerProcessInfo = new ProcessStartInfo("cmd.exe", "/c " + "C:\\Users\\John\\Downloads\\Program_Downloads\\For_FAU_PROJECT_MA_MMT\\MMT\\deploy\\mmt.bat");
        //CreateNoWindow = flase -> For now only for testing purposes
        mmtServerProcessInfo.CreateNoWindow = false;
        mmtServerProcessInfo.UseShellExecute = false;
        mmtServerProcessInfo.RedirectStandardError = true;
        mmtServerProcessInfo.RedirectStandardOutput = true;

        mmtServerProcess = Process.Start(mmtServerProcessInfo);
        */

        //   CommunicationEvents.AddPointEvent.AddListener(AddFactToMMT);
        //  CommunicationEvents.RemoveEvent.AddListener(RemoveFactFromMMT);   


    }
    /*
    void AddFactToMMT(RaycastHit hit, int id )
    {
        //send new fact to MMT
        UnityEngine.Debug.Log("add fact"+ id);
    }

    void RemoveFactFromMMT( int id)
    {
        UnityEngine.Debug.Log("remove fact"+ id);
    }
    */

}
