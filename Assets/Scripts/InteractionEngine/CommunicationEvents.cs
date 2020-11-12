using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public static class CommunicationEvents
{
    public class HitEvent : UnityEvent<RaycastHit>
    {

    }
    public class FactEvent : UnityEvent<Fact>
    {

    }

    public class MouseOverFactEvent : UnityEvent<Transform>
    {

    }
    public class ToolModeEvent : UnityEvent<int> {

    }

    public class ShinyEvent : UnityEvent<Fact> {

    }

    public class SignalEvent : UnityEvent {

    }

    public class AnimationEvent : UnityEvent<String> {

    }



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

    public static AnimationEvent parameterDisplayHint = new AnimationEvent();
    public static SignalEvent newAssignmentEvent = new SignalEvent();


    //------------------------------------------------------------------------------------
    //-------------------------------Global Variables-------------------------------------

    //Global List of Facts
    public static List<Fact> Facts = new List<Fact>();


    public static bool ServerRunning = true;
    public static string ServerAdress = "localhost:8085";
}
