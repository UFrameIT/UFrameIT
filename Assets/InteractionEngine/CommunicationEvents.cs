using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class CommunicationEvents
{
    public  class PointEvent : UnityEvent<RaycastHit,int>
    {

    }

    public class LineEvent : UnityEvent<Vector3, Vector3> {

    }

    public class HitEvent : UnityEvent<RaycastHit>
    {

    }
    public class FactEvent : UnityEvent<int>
    {

    }

    public class MouseOverFactEvent : UnityEvent<Transform>
    {

    }
    public class ToolModeEvent : UnityEvent<ToolMode> {

    }

    public static HitEvent TriggerEvent = new HitEvent();
    public static MouseOverFactEvent HighlightEvent = new MouseOverFactEvent();
    public static MouseOverFactEvent EndHighlightEvent = new MouseOverFactEvent();
    public static ToolModeEvent ToolModeChangedEvent = new ToolModeEvent();
    public static PointEvent AddPointEvent = new PointEvent();
    public static LineEvent AddLineEvent = new LineEvent();
    public static FactEvent RemoveEvent = new FactEvent();
    public static ToolMode ActiveToolMode { get; set; }
}
