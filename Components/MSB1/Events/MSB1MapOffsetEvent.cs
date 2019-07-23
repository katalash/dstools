using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 1/Events/Map Offset")]
public class MSB1MapOffsetEvent : MSB1Event
{
    /// <summary>
    /// Position of the map offset.
    /// </summary>
    public Vector3 Position;

    public override void SetEvent(MSB1.Event bevt)
    {
        var evt = (MSB1.Event.MapOffset)bevt;
        setBaseEvent(evt);
        Position = new Vector3(evt.Position.X, evt.Position.Y, evt.Position.Z);
    }

    public override MSB1.Event Serialize(GameObject parent)
    {
        var evt = new MSB1.Event.MapOffset();
        _Serialize(evt, parent);
        var pos = new System.Numerics.Vector3();
        pos.X = Position.x;
        pos.Y = Position.y;
        pos.Z = Position.z;
        evt.Position = pos;
        return evt;
    }
}
