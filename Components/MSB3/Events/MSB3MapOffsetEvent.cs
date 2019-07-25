using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 3/Events/Map Offset")]
public class MSB3MapOffsetEvent : MSB3Event
{
    /// <summary>
    /// Position of the map offset.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Rotation of the map offset.
    /// </summary>
    public float Degree;

    public override void SetEvent(MSB3.Event bevt)
    {
        var evt = (MSB3.Event.MapOffset)bevt;
        setBaseEvent(evt);
        Position = new Vector3(evt.Position.X, evt.Position.Y, evt.Position.Z);
        Degree = evt.Degree;
    }

    public override MSB3.Event Serialize(GameObject parent)
    {
        var evt = new MSB3.Event.MapOffset(parent.name);
        _Serialize(evt, parent);
        evt.Position = new System.Numerics.Vector3(Position.x, Position.y, Position.z);
        evt.Degree = Degree;
        return evt;
    }
}
