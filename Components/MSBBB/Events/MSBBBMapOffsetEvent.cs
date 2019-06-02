using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Bloodborne/Events/Map Offset")]
public class MSBBBMapOffsetEvent : MSBBBEvent
{
    /// <summary>
    /// Position of the map offset.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// Rotation of the map offset.
    /// </summary>
    public float Degree;

    public override void SetEvent(MSBBB.Event bevt)
    {
        setBaseEvent(bevt);
        var evt = (MSBBB.Event.MapOffset)bevt;
        Position = new Vector3(evt.Position.X, evt.Position.Y, evt.Position.Z);
        Degree = evt.Degree;
    }

    public override MSBBB.Event Serialize(GameObject parent)
    {
        var evt = new MSBBB.Event.MapOffset(parent.name);
        _Serialize(evt, parent);
        evt.Position = new System.Numerics.Vector3(Position.x, Position.y, Position.z);
        evt.Degree = Degree;
        return evt;
    }
}
