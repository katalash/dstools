using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Events/Walk Route")]
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

    public override void SetEvent(MSB3.Event bevt)
    {
        var evt = (MSB3.Event.WalkRoute)bevt;
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        WalkPointNames = evt.WalkPointNames;
    }

    public override MSB3.Event Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.WalkRoute(parent.name);
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        for (int i = 0; i < 32; i++)
        {
            if (i >= WalkPointNames.Length)
            {
                evt.WalkPointNames[i] = null;
                continue;
            }
            evt.WalkPointNames[i] = (WalkPointNames[i] == "") ? null : WalkPointNames[i];
        }
        return evt;
    }
}
