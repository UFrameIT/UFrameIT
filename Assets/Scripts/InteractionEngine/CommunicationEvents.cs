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

    // Global Level-List of Facts
    public static FactOrganizer LevelFacts = new FactOrganizer(true);
    public static FactOrganizer SolutionManager = new FactOrganizer(false);
    //TODO? [SolutionManager, List<[HashSet<string>, FactComparer]>]
    public static List<Fact> Solution = new List<Fact>();

    public static bool ServerRunning = true;
    public static string ServerAdress = "localhost:8085";

    // Configs
    public static bool VerboseURI = false;

    // TODO! avoid tree traversel with name
    public static string CreatePathToFile(out bool file_exists,  string name, string format = null, Type type = null)
    {
        string ending = ".dat";
        if(!string.IsNullOrEmpty(format))
            switch (format)
            {
                case "JSON":
                    ending = ".JSON";
                    break;
                default:
                    break;
            }

        string path = Application.persistentDataPath;
        if (type != null)
        {
            if (typeof(FactOrganizer).IsAssignableFrom(type))
                path = System.IO.Path.Combine(path, "FactStateMachines");
            else
                { }

            System.IO.Directory.CreateDirectory(path);
        }

        path = System.IO.Path.Combine(path, name + ending);
        file_exists = System.IO.File.Exists(path);

        return path;
    }
}
