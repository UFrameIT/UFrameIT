using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class CommunicationEvents
{
    public  class PointEvent : UnityEvent<RaycastHit,int>
    {

    }
    public class HitEvent : UnityEvent<RaycastHit>
    {

    }

    public static HitEvent TriggerEvent = new HitEvent();
    public static PointEvent AddEvent = new PointEvent();
    public static PointEvent RemoveEvent = new PointEvent();

}
