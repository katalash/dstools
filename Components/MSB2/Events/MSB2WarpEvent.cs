using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

[AddComponentMenu("Dark Souls 2/Events/Warp")]
public class MSB2WarpEvent : MSB2Event
{
    /// <summary>
    /// Unknown.
    /// </summary>
    public int UnkT00;

    /// <summary>
    /// Position of the map offset.
    /// </summary>
    public Vector3 Position;

    public override void SetEvent(MSB2.Event bevt)
    {
        var evt = (MSB2.Event.Warp)bevt;
        setBaseEvent(evt);
        UnkT00 = evt.UnkT00;
        Position = new Vector3(evt.Position.X, evt.Position.Y, evt.Position.Z);
    }

    public override MSB2.Event Serialize(GameObject parent)
    {
        var evt = new MSB2.Event.Warp();
        _Serialize(evt, parent);
        evt.UnkT00 = UnkT00;
        var pos = new System.Numerics.Vector3();
        pos.X = Position.x;
        pos.Y = Position.y;
        pos.Z = Position.z;
        evt.Position = pos;
        return evt;
    }
}
