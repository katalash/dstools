using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulsFormats;

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

    public void SetEvent(MSB3.Event.MapOffset evt)
    {
        setBaseEvent(evt);
        Position = new Vector3(evt.Position.X, evt.Position.Y, evt.Position.Z);
        Degree = evt.Degree;
    }
}
