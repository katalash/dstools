using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;
using MeowDSIO.DataTypes.MSB;
using MeowDSIO.DataTypes.MSB.EVENT_PARAM_ST;

[AddComponentMenu("Dark Souls 1/Events/Map Offset")]
public class MSB1MapOffsetEvent : MSB1Event
{
    /// <summary>
    /// Position of the map offset.
    /// </summary>
    public Vector3 Position;

    public void SetEvent(MsbEventMapOffset evt)
    {
        setBaseEvent(evt);
        Position = new Vector3(evt.X, evt.Y, evt.Z);
    }

    public MsbEventMapOffset Serialize(GameObject parent)
    {
        var evt = new MsbEventMapOffset();
        _Serialize(evt, parent);
        evt.X = Position.x;
        evt.Y = Position.y;
        evt.Z = Position.z;
        return evt;
    }
}
