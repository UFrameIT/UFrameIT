using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public static class CommunicationEvents
{
    /*  public  class PointEvent : UnityEvent<RaycastHit,int>
      {

      }

      public class LineEvent : UnityEvent<int, int, int> {

      }



      public class FactEvent : UnityEvent<int>
      {

      }*/
    public class HitEvent : UnityEvent<RaycastHit>
    {

    }
    public class FactEvent : UnityEvent<Fact>
    {

    }
  



    public class MouseOverFactEvent : UnityEvent<Transform>
    {

    }
    public class ToolModeEvent : UnityEvent<ToolMode> {

    }

    public static HitEvent TriggerEvent = new HitEvent();

    public static ToolModeEvent ToolModeChangedEvent = new ToolModeEvent();
    /*
    public static FactEvent AddPointEvent = new FactEvent();
    public static FactEvent AddLineEvent = new FactEvent();
    public static FactEvent AddAngleEvent = new FactEvent();
    */
    public static FactEvent AddFactEvent = new FactEvent();
    public static FactEvent RemoveFactEvent = new FactEvent();

    public static ToolMode ActiveToolMode { get; set; }

    public static List<Fact> Facts = new List<Fact>();

    // public static MouseOverFactEvent HighlightEvent = new MouseOverFactEvent();
    //public static MouseOverFactEvent EndHighlightEvent = new MouseOverFactEvent();

}
