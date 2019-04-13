using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Sekiro/Events/Map Offset")]
public class MSBSMapOffsetEvent : MSBSEvent
{

    public Vector3 Position;
    public float Degree;

    public void SetEvent(MSBS.Event.MapOffset evt)
    {
        setBaseEvent(evt);
        Position = new UnityEngine.Vector3(evt.Position.X, evt.Position.Y, evt.Position.Z);
        Degree = evt.Degree;
    }

    public MSBS.Event.MapOffset Serialize(GameObject parent)
    {
        var evt = new MSBS.Event.MapOffset();
        _Serialize(evt, parent);
        evt.Position = new System.Numerics.Vector3(Position.x, Position.y, Position.z);
        evt.Degree = Degree;
        return evt;
    }
}
