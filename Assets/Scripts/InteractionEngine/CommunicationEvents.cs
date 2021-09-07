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

    public static AnimationEvent ScrollFactHintEvent = new AnimationEvent();
    public static FactEvent AnimateExistingFactEvent = new FactEvent();
    public static FactEvent AnimateNonExistingFactEvent = new FactEvent();
    public static AnimationEventWithUris HintAvailableEvent = new AnimationEventWithUris();


    //------------------------------------------------------------------------------------
    //-------------------------------Global Variables-------------------------------------
    // TODO! move to GlobalStatic/Behaviour


    public static bool ServerRunning = true;
    public static string ServerAdress = "localhost:8085";

    // Configs
    public static bool VerboseURI = false;

    public enum Directories
    {
        Stages,
        FactStateMachines,
        ValidationSets
    }

    public static string CreateHierarchiePath(List<Directories> hierarchie, string prefix = "", string postfix = "")
    {
        foreach (var dir in hierarchie)
            prefix = System.IO.Path.Combine(prefix, dir.ToString());

        return System.IO.Path.Combine(prefix, postfix);
    }

    // TODO! avoid tree traversel with name
    public static string CreatePathToFile(out bool file_exists,  string name, string format = null, List<Directories> hierarchie = null, bool use_install_folder = false)
    {
        string ending = "";
        if(!string.IsNullOrEmpty(format))
            switch (format)
            {
                case "JSON":
                    ending = ".JSON";
                    break;
                default:
                    break;
            }

        string path = use_install_folder ? Application.dataPath : Application.persistentDataPath;
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
