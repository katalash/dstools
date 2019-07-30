
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Events/Map Offset")]
public class MSB2MapOffsetEvent : MSB2Event
{
    /// <summary>
    /// Position of the map offset.
    /// </summary>
    public Vector3 Position;

    public override void SetEvent(MSB2.Event bevt)
    {
        var evt = (MSB2.Event.MapOffset)bevt;
        setBaseEvent(evt);
        Position = new Vector3(evt.Translation.X, evt.Translation.Y, evt.Translation.Z);
    }

    public override MSB2.Event Serialize(GameObject parent)
    {
        var evt = new MSB2.Event.MapOffset();
        _Serialize(evt, parent);
        var pos = new System.Numerics.Vector3();
        pos.X = Position.x;
        pos.Y = Position.y;
        pos.Z = Position.z;
        evt.Translation = pos;
        return evt;
    }
}
