using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

public class MSB3WalkRouteEvent : MSB3Event
{
    /// <summary>
    /// Unknown; probably some kind of route type.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// List of points in the route.
    /// </summary>
    public string[] WalkPointNames;

    public void SetEvent(MSB3.Event.WalkRoute evt)
    {
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        WalkPointNames = evt.WalkPointNames;
    }
}
