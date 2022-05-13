using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public static class CommunicationEvents
{
    public class HitEvent : UnityEvent<RaycastHit> { }

    public class FactEvent : UnityEvent<Fact> { }

    public class MouseOverFactEvent : UnityEvent<Transform> { }

    public class ToolModeEvent : UnityEvent<int> { }

    public class ShinyEvent : UnityEvent<Fact> { }

    public class SignalEvent : UnityEvent { }

    public class AnimationEvent : UnityEvent<GameObject, string> { }

    public class AnimationEventWithUris : UnityEvent<List<string>> { }



    public static HitEvent SnapEvent = new HitEvent();
    public static HitEvent TriggerEvent = new HitEvent();

    public static ToolModeEvent ToolModeChangedEvent = new ToolModeEvent();
    public static FactEvent AddFactEvent = new FactEvent();
    public static FactEvent RemoveFactEvent = new FactEvent();

    public static ShinyEvent PushoutFactEvent = new ShinyEvent();
    public static ShinyEvent PushoutFactEndEvent = new ShinyEvent();
    public static ShinyEvent PushoutFactFailEvent = new ShinyEvent();

    public static SignalEvent gameSucceededEvent = new SignalEvent();
    public static SignalEvent gameNotSucceededEvent = new SignalEvent();
    public static SignalEvent LevelReset = new SignalEvent();
    public static SignalEvent NewAssignmentEvent = new SignalEvent();
    //public static SignalEvent ReloadConfigToUI = new SignalEvent();

    public static AnimationEvent ScrollFactHintEvent = new AnimationEvent();
    public static FactEvent AnimateExistingFactEvent = new FactEvent();
    public static FactEvent AnimateNonExistingFactEvent = new FactEvent();
    public static AnimationEventWithUris HintAvailableEvent = new AnimationEventWithUris();


    //------------------------------------------------------------------------------------
    //-------------------------------Global Variables-------------------------------------
    // TODO! move to GlobalStatic/Behaviour


    public static bool ServerRunning = true;
    public static bool ServerRunning_test = true;
    public static string ServerPortDefault = "8085";
    //public static string ServerAdress = "http://localhost:8085"; //need "http://" 
    public static string ServerAdress = "http://10.231.4.95:8085";
    public static string ServerAddress1 = "localhost:8085";
    public static string ServerAddress2 = "10.231.4.95:8085";

    
    public static bool takeNewToolID = false; //0=no, 1=instead, 2=both
    public static int ToolID_new;
    public static int ToolID_selected;//Script
    

    public static string lastIP = "";
    public static string newIP = "";
    public static string IPslot1 = "";
    public static string IPslot2 = "http://10.231.4.95:8085";
    public static string IPslot3 = "10.231.4.95:8085";
    public static string selecIP = "";
    
    public static int[] ServerRunningA = new int[7] { 0, 0, 0, 0, 0, 0, 0 }; //other, lastIP, newIP, IP1, IP2, IP3, selecIP} //0: offline, 1: Checking, 2: online, 3: NoNetworkAddress;
    public static bool[] ServerRunningA_test = new bool[7] { false, false, false, false, false, false, false }; //other, lastIP, newIP, IP1, IP2, IP3, selecIP}
    public static double IPcheckGeneration = 0;
    public static int CheckNetLoop = 1;
    
    public static bool autoOSrecognition = true;
    //int Opsys =1 Android.
    //int Opsys =0 Windows;
    //public static int Opsys_Default = 0;

    public static int Opsys = 1; //Script
    public static bool CursorVisDefault = true; //Script.






    // Configs
    public static bool VerboseURI = false;


    public enum Directories
    {
        Stages,
        ValidationSets,
        FactStateMachines,
    }

    public static string CreateHierarchiePath(List<Directories> hierarchie, string prefix = "", string postfix = "")
    {
        foreach (var dir in hierarchie)
            prefix = System.IO.Path.Combine(prefix, dir.ToString());

        return System.IO.Path.Combine(prefix, postfix);
    }

    // TODO! avoid tree traversel with name
    public static string CreatePathToFile(out bool file_exists, string name, string format = null, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        string ending = "";
        string path; 
        if (!string.IsNullOrEmpty(format))
            switch (format)
            {
                case "JSON":
                    ending = ".JSON";
                    break;
                default:
                    break;
            }

        //int Opsys =1 Android;
        //int Opsys =0 Windows;
        //is set above;
        switch (Opsys)
        {
            case 0:
                path = use_install_folder ? Application.dataPath : Application.persistentDataPath;
                
                
                if (hierarchie != null)
                {
                    path = CreateHierarchiePath(hierarchie, path);
                    System.IO.Directory.CreateDirectory(path);
                }

                path = System.IO.Path.Combine(path, name + ending);
                file_exists = System.IO.File.Exists(path);

                return path;

            case 1:

                path = Application.persistentDataPath;
                if (hierarchie != null)
                {
                    path = CreateHierarchiePath(hierarchie, path);
                    System.IO.Directory.CreateDirectory(path);
                }
                path = System.IO.Path.Combine(path, name + ending);
                file_exists = System.IO.File.Exists(path);

                return path;

            default:
                path = use_install_folder ? Application.dataPath : Application.persistentDataPath;
                if (hierarchie != null)
                {
                    path = CreateHierarchiePath(hierarchie, path);
                    System.IO.Directory.CreateDirectory(path);
                }

                path = System.IO.Path.Combine(path, name + ending);
                file_exists = System.IO.File.Exists(path);

                return path;


        }

    }
}
