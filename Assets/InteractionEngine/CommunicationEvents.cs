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

    public class ShinyEvent : UnityEvent<Fact> {

    }



    public static HitEvent SnapEvent = new HitEvent();
    public static HitEvent TriggerEvent = new HitEvent();

    public static ToolModeEvent ToolModeChangedEvent = new ToolModeEvent();
    /*
    public static FactEvent AddPointEvent = new FactEvent();
    public static FactEvent AddLineEvent = new FactEvent();
    public static FactEvent AddAngleEvent = new FactEvent();
    */
    public static FactEvent AddFactEvent = new FactEvent();
    public static FactEvent RemoveFactEvent = new FactEvent();

    //public static MouseOverFactEvent HighlightEvent = new MouseOverFactEvent();
    //public static MouseOverFactEvent EndHighlightEvent = new MouseOverFactEvent();

    public static ShinyEvent StartLineDrawingEvent = new ShinyEvent();
    public static ShinyEvent StopLineDrawingEvent = new ShinyEvent();
    public static ShinyEvent StartCurveDrawingEvent = new ShinyEvent();
    public static ShinyEvent StopCurveDrawingEvent = new ShinyEvent();
    //Event for stopping all previews -> Made When ToolMode is changed
    public static ShinyEvent StopPreviewsEvent = new ShinyEvent();
    public static ShinyEvent PushoutFactEvent = new ShinyEvent();




    //------------------------------------------------------------------------------------
    //-------------------------------Global Variables-------------------------------------
    //Global ActiveToolMode
    public static ToolMode ActiveToolMode { get; set; }


    //Global List of Facts
    public static List<Fact> Facts = new List<Fact>();

}
